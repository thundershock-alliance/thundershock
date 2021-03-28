﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Thundershock
{
    public class EntryPoint
    {
        private static Dictionary<string, Type> _entryPoints = new Dictionary<string, Type>();
        private static App _current;
        private static Assembly _entryAssembly;

        public static Assembly EntryAssembly => _entryAssembly;
        public static App CurrentApp => _current;
        
        public static void RegisterApp(string appName, Type type)
        {
            if (string.IsNullOrWhiteSpace(appName))
                throw new FormatException(nameof(appName));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!typeof(App).IsAssignableFrom(type))
                throw new InvalidOperationException(
                    "The specified type is not a Thundershock.App class and cannot be used as a Thundershock entry-point.");

            if (type.GetConstructor(Type.EmptyTypes) == null)
                throw new InvalidOperationException(
                    "The given Thundershock app class is not constructable (it doesn't have a parameterless instance constructor) and cannot be used as an entry-point.");

            if (_entryPoints.ContainsKey(appName))
                throw new InvalidOperationException("The given application name is already registered.");
            
            _entryPoints.Add(appName, type);
        }

        private static EntryArgs GetEntryArgs(string cmd, string[] args)
        {
            // result
            var entryArgs = new EntryArgs();
            
            // usage string builder
            var builder = new UsageStringBuilder(cmd);

            foreach (var key in _entryPoints.Keys)
                builder.AddAction(key);

            // setting overrides
            builder.AddFlag('m', "mute-audio", "Completely mute all audio players.", x => entryArgs.MuteAudio = x);
            builder.AddFlag('C', "skip-config", "Skip loading the game's user configuration.",
                x => entryArgs.SkipConfig = x);
            builder.AddFlag('p', "no-postprocessing", "Shut off the Thundershock post-processor.",
                x => entryArgs.DisablePostProcessor = x);
            builder.AddFlag('v', "verbose", "Verbose engine logging to console.", x => entryArgs.Verbose = x);
            builder.AddFlag('w', "wipe-user-data",
                "COMPLETELY WIPE ALL USER DATA FOR THE GAME BEING RUN. Thundershock will not ask your permission before doing so.", x => entryArgs.WipeConfig = x);
            builder.AddFlag('l', "layout-debug", "GUI system layout debugger enable", x => entryArgs.LayoutDebug = x);
            
            // Apply the command-line arguments to the usage string.
            builder.Apply(args);

            return entryArgs;
        }
        
        public static void Run<T>(string[] args) where T : App, new()
        {
            // Retrieve the entry-point assembly. This is important for building the docopt usage string.
            var entryPointAssembly = Assembly.GetEntryAssembly();
            _entryAssembly = entryPointAssembly;
            
            // Get the file name.
            var entryPointFileName = Path.GetFileName(entryPointAssembly.Location);

            // Get entry arguments
            var entryArgs = GetEntryArgs(entryPointFileName, args);
            
            // the app we're going to run
            var app = null as App;
            
            // Did the entry args specify an entry point?
            if (!string.IsNullOrWhiteSpace(entryArgs.AppEntry))
            {
                // create the app specified
                app = (App) Activator.CreateInstance(_entryPoints[entryArgs.AppEntry], null);
            }
            else
            {
                // Create the default app.
                app = new T();
            }
            
            // *a distant rumble occurs in the distance, followed by a flash of light*
            Bootstrap(app);

            _entryAssembly = null;
        }

        private static void Bootstrap(App app)
        {
            if (_current != null)
                throw new InvalidOperationException("Failed to bootstrap the app. Thundershock is already running.");

            // bind this thundershock app to this instance of thundershock.
            _current = app;

            // Create a new MonoGame game for Thundershock to run inside.
            using var mg = new MonoGameLoop(app);
            
            // it's at this point we should register any core components that need to be available before the app starts.
            // Right now, that's nothing.
            
            // To get the app to run, just run MonoGame. The MonoGame loop will finish bootstrapping Thundershock by design.
            mg.Run();
            
            // The above method blocks until MonoGame tears itself down successfully. If we get this far, we can unbind the app.
            _current = null;
        }

        private class EntryArgs
        {
            public bool SkipConfig;
            public bool MuteAudio;
            public bool DisablePostProcessor;
            public bool Verbose;
            public bool WipeConfig;
            public bool LayoutDebug;

            public string AppEntry;
        }
    }
}
