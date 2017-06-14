using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Actors.Navis;
using Megaman.Chips;
using Megaman.Overworld;
using Megaman.Projectiles;
using Microsoft.CodeAnalysis;
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

            scriptOptions = scriptOptions.WithSourceResolver(new SourceFileResolver(new List<string>(), AppContext.BaseDirectory));

            string s = new StreamReader(script).ReadToEnd();

            Task<ScriptState<object>> results = null;

            try
            {
                results = CSharpScript.RunAsync(@s, scriptOptions);
            }
            catch (Microsoft.CodeAnalysis.Scripting.CompilationErrorException e)
            {
                Log("Error loading " + script + " :\n" + e);
            }

            return results.Result;

        }

        public static void Log(string message)
        {
            System.IO.File.AppendAllText("log.txt", message + "\n");
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
            filepath = Game.modulePath + filepath;

            if (File.Exists(filepath))
            {
                FileStream fileStream = new FileStream(filepath, FileMode.Open);
                Texture2D image = Texture2D.FromStream(Game.graphics.GraphicsDevice, fileStream);

                fileStream.Close();
                return image;
            }
            else
            {
                Log("Cannot find file" + filepath);
                return null;
            }
        }

        public static List<string> getFilesFromFolder(string path)
        {
            path = Game.modulePath + path;

            return Directory.GetFiles(path).Select(Path.GetFileNameWithoutExtension).ToList();
        }
    }
}