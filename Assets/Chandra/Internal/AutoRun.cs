#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Chandra.Internal.Attributes;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chandra.Internal
{
    [InitializeOnLoad]
    public  class AutoRun : IPreprocessBuildWithReport
    {

        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            OnBuildCheck();
        }

        static AutoRun()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBuildCheck;
            EditorApplication.playModeStateChanged += OnEditorApplicationOnplayModeStateChanged;
        }

        private static void OnEditorApplicationOnplayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode) return;
            var valid = ValidateAttributes();
            if (valid == null) return;
            EditorApplication.ExitPlaymode();
            throw valid;
        }
        private static void OnBuildCheck()
        {
            var valid = ValidateAttributes();
            if (valid != null)
            {
                throw valid;
            }
        }
        //Makes the build process fail
        private static BuildFailedException ValidateAttributes()
        {
            MethodInfo[] methods = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t =>
                    t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                 BindingFlags
                                     .Instance)) // Include instance methods in the search so we can show the developer an error instead of silently not adding instance methods to the dictionary
                .Where(m => m.GetCustomAttributes(typeof(MessageHandler), false).Length > 0)
                .ToArray();
            var nonStaticMethods = methods.Where(method => !method.IsStatic).ToArray();
            StringBuilder build = new StringBuilder();
            foreach (var method in nonStaticMethods)
            {
                build.Append(
                    $"{method.Name} uses the {nameof(MessageHandler)} attribute but it is not a static method at (at Assets/Chandra/Internal/OnBuildHandler.cs:32)");
                build.AppendLine();
            }

            if (nonStaticMethods.Length > 0)
            {
                return new BuildFailedException(
                    $"{nonStaticMethods.Length} METHODS implement the {nameof(MessageHandler)} and are not static:\n" +
                    build);
            }

            return null;
        }

    }
}
#endif
