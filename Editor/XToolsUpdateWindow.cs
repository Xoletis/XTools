using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Xoletis.EditorTools
{
    public class XToolsUpdateWindow : EditorWindow
    {
        private const string GitUrl = "https://github.com/Xoletis/XTools.git";
        private const string TagsApiUrl = "https://api.github.com/repos/Xoletis/XTools/tags";

        private enum CheckState { Idle, Checking, UpToDate, UpdateAvailable, Error }

        private string _installedVersion = "-";
        private string _latestVersion = "-";
        private string _changelogText = "";
        private CheckState _state = CheckState.Idle;
        private string _errorMessage;
        private Vector2 _changelogScroll;

        private UnityWebRequest _tagsRequest;
        private AddRequest _addRequest;

        [MenuItem("Tools/XTools/Update")]
        private static void Open()
        {
            var window = GetWindow<XToolsUpdateWindow>("XTools Update");
            window.minSize = new Vector2(360, 420);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshInstalledInfo();
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void RefreshInstalledInfo()
        {
            var packageInfo = PackageInfo.FindForAssembly(typeof(XToolsUpdateWindow).Assembly);
            if (packageInfo == null)
            {
                return;
            }

            _installedVersion = packageInfo.version;
            LoadLocalChangelog(packageInfo.resolvedPath);
        }

        private void LoadLocalChangelog(string resolvedPath)
        {
            try
            {
                var changelogPath = Path.Combine(resolvedPath, "CHANGELOG.md");
                _changelogText = File.Exists(changelogPath)
                    ? File.ReadAllText(changelogPath)
                    : "No CHANGELOG.md found.";
            }
            catch (Exception e)
            {
                _changelogText = $"Failed to read CHANGELOG.md: {e.Message}";
            }
        }

        private void CheckForUpdates()
        {
            _state = CheckState.Checking;
            _errorMessage = null;
            _tagsRequest = UnityWebRequest.Get(TagsApiUrl);
            _tagsRequest.SetRequestHeader("User-Agent", "XTools-Updater");
            _tagsRequest.SendWebRequest();
        }

        private void OnEditorUpdate()
        {
            if (_tagsRequest != null && _tagsRequest.isDone)
            {
                HandleTagsResponse(_tagsRequest);
                _tagsRequest.Dispose();
                _tagsRequest = null;
                Repaint();
            }

            if (_addRequest != null && _addRequest.IsCompleted)
            {
                if (_addRequest.Status == StatusCode.Success)
                {
                    _state = CheckState.UpToDate;
                    RefreshInstalledInfo();
                }
                else
                {
                    _state = CheckState.Error;
                    _errorMessage = _addRequest.Error?.message ?? "Update failed.";
                }

                _addRequest = null;
                Repaint();
            }
        }

        private void HandleTagsResponse(UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                _state = CheckState.Error;
                _errorMessage = request.error;
                return;
            }

            try
            {
                var wrapped = "{\"items\":" + request.downloadHandler.text + "}";
                var list = JsonUtility.FromJson<GitHubTagListWrapper>(wrapped);
                var best = list.items?
                    .Select(t => ParseVersion(t.name))
                    .Where(v => v != null)
                    .OrderByDescending(v => v)
                    .FirstOrDefault();

                if (best == null)
                {
                    _state = CheckState.Error;
                    _errorMessage = "No version tags found in the repository.";
                    return;
                }

                _latestVersion = best.ToString();
                var current = ParseVersion(_installedVersion);
                _state = current != null && best.CompareTo(current) > 0
                    ? CheckState.UpdateAvailable
                    : CheckState.UpToDate;
            }
            catch (Exception e)
            {
                _state = CheckState.Error;
                _errorMessage = $"Failed to parse tags: {e.Message}";
            }
        }

        private static Version ParseVersion(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return null;
            }

            var cleaned = raw.TrimStart('v', 'V');
            return Version.TryParse(cleaned, out var v) ? v : null;
        }

        private void InstallLatest()
        {
            _addRequest = Client.Add($"{GitUrl}#{_latestVersion}");
            _state = CheckState.Checking;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("XTools Update", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Installed version", _installedVersion);
            EditorGUILayout.LabelField("Latest version", _state == CheckState.Checking ? "Checking..." : _latestVersion);

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Check for Updates"))
                {
                    CheckForUpdates();
                }

                using (new EditorGUI.DisabledScope(_state != CheckState.UpdateAvailable))
                {
                    if (GUILayout.Button("Install Update"))
                    {
                        InstallLatest();
                    }
                }
            }

            switch (_state)
            {
                case CheckState.UpToDate:
                    EditorGUILayout.HelpBox("XTools is up to date.", MessageType.Info);
                    break;
                case CheckState.UpdateAvailable:
                    EditorGUILayout.HelpBox($"A new version ({_latestVersion}) is available.", MessageType.Warning);
                    break;
                case CheckState.Error:
                    EditorGUILayout.HelpBox(_errorMessage ?? "An error occurred.", MessageType.Error);
                    break;
                case CheckState.Checking:
                    EditorGUILayout.HelpBox("Working...", MessageType.None);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Changelog", EditorStyles.boldLabel);
            _changelogScroll = EditorGUILayout.BeginScrollView(_changelogScroll, GUILayout.ExpandHeight(true));
            EditorGUILayout.TextArea(_changelogText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        [Serializable]
        private class GitHubTag
        {
            public string name;
        }

        [Serializable]
        private class GitHubTagListWrapper
        {
            public GitHubTag[] items;
        }
    }
}
