#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace CustomResourceRegister
{
	[Tool]
	public class Plugin : EditorPlugin
	{
		private readonly List<string> _scripts = new List<string>();
		private Control _control;

		private Control _gui => GetEditorInterface().GetBaseControl();

		public override void _EnterTree()
		{

			Settings.Init();
			RegisterCustomClasses();
			_control = CreateBottomMenuControl();
			AddControlToBottomPanel(_control, "CRR");
		}

		public override void _ExitTree()
		{
			UnregisterCustomClasses();
			RemoveControlFromBottomPanel(_control);
			_control = null;
		}

		private void RegisterCustomClasses()
		{
			_scripts.Clear();

			var file = new File();

			foreach (var type in GetCustomTypes())
			{
				var path = ClassPath(type);
				if (!file.FileExists(path))
					continue;
				CSharpScript script = (CSharpScript)GD.Load<Script>(path);
				if (script == null)
					continue;

				string typeBase = script.New().GetType().BaseType.Name;

				AddCustomType($"{Settings.ClassPrefix}{type.Name}", typeBase, script, _gui.GetIcon(typeBase, "EditorIcons"));
				GD.Print($"Register custom type: {type.Name} extends {typeBase} -> {path}");
				_scripts.Add(type.Name);
			}
		}

		private static string ClassPath(Type type)
		{
			return $"{Settings.ScriptsFolder}{type.Namespace?.Replace(".", "/") ?? ""}{type.Name}.cs";
		}

		private static IEnumerable<Type> GetCustomTypes()
		{
			var assembly = Assembly.GetAssembly(typeof(Plugin));
			return assembly.GetTypes().Where(t => !t.IsAbstract && (t.IsSubclassOf(typeof(Godot.Object))));
		}

		private void UnregisterCustomClasses()
		{
			foreach (var script in _scripts)
			{
				RemoveCustomType(script);
				GD.Print($"Unregister custom type: {script}");
			}

			_scripts.Clear();
		}

		private Control CreateBottomMenuControl()
		{
			Control container = new GridContainer()
			{
				RectMinSize = new Vector2(100, 100),
				SizeFlagsHorizontal = (int) Control.SizeFlags.Fill | (int) Control.SizeFlags.Expand,
				SizeFlagsVertical = (int) Control.SizeFlags.Fill | (int) Control.SizeFlags.Expand
			};

			Control button = new Button {
				Text = "Refresh",
				SizeFlagsHorizontal = (int) Control.SizeFlags.Fill | (int) Control.SizeFlags.Expand,
				SizeFlagsVertical = (int) Control.SizeFlags.Fill | (int) Control.SizeFlags.Expand
			};
			button.Connect("pressed", this, nameof(OnRefreshPressed));
			container.AddChild(button);
			return container;
		}

		private void OnRefreshPressed()
		{
			UnregisterCustomClasses();
			RegisterCustomClasses();
		}
	}
}
#endif