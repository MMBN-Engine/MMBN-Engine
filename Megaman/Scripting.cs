using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Actors.Navis;
using Megaman.Chips;
using Megaman.Overworld;
using Megaman.Projectiles;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CustomExtensions;
using System.IO;

namespace Megaman
{
    /// <summary>
    /// This contains code used for scripting and loading models
    /// </summary>
    public class Scripting
    {
        public Scripting()
        {
        }

        public static ScriptState parse(string script)
        {
            ScriptOptions scriptOptions = ScriptOptions.Default.WithReferences(typeof(MegaMan).Assembly);

            script = new StreamReader(script).ReadToEnd();

            ScriptState state = null;

            CSharpScript.RunAsync(@script, scriptOptions).ContinueWith(s => state = s.Result).Wait();
            return state;
        }

        public static object getScriptValue(string field, ScriptVariable v)
        {
            return v.Value.GetType().GetProperty(field).GetValue(v.Value);
        }

        public static void equateFields(object o, ScriptVariable v)
        {
            foreach (PropertyInfo p in v.Value.GetType().GetProperties())
            {
                o.GetType().GetField(p.Name).SetValue(o, getScriptValue(p.Name, v));
            }
        }

        public static Texture2D loadImage(string filepath)
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Open);
            Texture2D image = Texture2D.FromStream(Game.graphics.GraphicsDevice, fileStream);

            fileStream.Close();
            return image;
        }

        public static List<string> getFilesFromFolder(string path)
        {
            return Directory.GetFiles(path).Select(Path.GetFileNameWithoutExtension).ToList();
        }
    }
}