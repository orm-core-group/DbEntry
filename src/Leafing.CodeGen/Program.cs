using System;
using System.IO;
using Leafing.Core;
using System.Reflection;

namespace Leafing.CodeGen
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                Process(args);
                return 0;
            }
            catch (ArgsErrorException ex)
            {
                if (ex.ReturnCode != 0)
                {
                    Console.WriteLine(ex.Message);
                }
                ShowHelp();
                return ex.ReturnCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 999;
        }

        private static void Process(string[] args)
        {
            if(args.Length > 0 && args[0].ToLower() == "m")
            {
                if(args.Length == 1)
                {
                    ShowTableList();
                }
                else
                {
                    GenerateModelFromDatabase(args[1]);
                }
                return;
            }

            if (args.Length < 2)
            {
                throw new ArgsErrorException(0, null);
            }

            var fileName = Path.GetFullPath(args[1]);

            if (!File.Exists(fileName))
            {
                throw new ArgsErrorException(2, "The file you input doesn't exist!");
            }

			switch (args [0].ToLower ()) {
			case "a":
			case "asp":
			case "aspnet":
				if (!SearchClasses (fileName, args.Length)) {
					GenerateAspNetTemplate (fileName, args [2]);
				}
				break;
			case "ra":
			case "action":
				if (!SearchClasses (fileName, args.Length)) {
					if (args.Length >= 4) {
						var gen = new MvcActionGenerator (fileName, args [2], args [3]);
						string s = gen.ToString ();
						Console.WriteLine (s);
					} else {
						throw new ArgsErrorException (3, "Need class name and action name.");
					}
				}
				break;
			case "rv":
			case "view":
				if (!SearchClasses (fileName, args.Length)) {
					if (args.Length >= 4) {
						string mpn = args.Length >= 5 ? args [4] : null;
						var gen = new MvcViewGenerator (fileName, args [2], args [3], mpn);
						string s = gen.ToString ();
						Console.WriteLine (s);
					} else {
						throw new ArgsErrorException (4, "Need class name and view name.");
					}
				}
				break;
			case "fn":
			case "fullname":
				var assembly = Assembly.LoadFile (args [1]);
				Console.WriteLine (assembly.FullName);
				break;
			default:
				throw new ArgsErrorException (0, null);
			}
        }

        //private static void GenerateAssembly(string fileName)
        //{
        //    ObjectInfo.GetInstance(typeof (LeafingEnum));
        //    ObjectInfo.GetInstance(typeof (LeafingLog));
        //    ObjectInfo.GetInstance(typeof (DbEntryMembershipUser));
        //    ObjectInfo.GetInstance(typeof (DbEntryRole));
        //    ObjectInfo.GetInstance(typeof (LeafingSetting));
        //    Helper.EnumTypes(fileName, true, t =>
        //    {
        //        ObjectInfo.GetInstance(t);
        //        return true;
        //    });
        //    MemoryAssembly.Instance.Save();
        //}

        private static void GenerateAspNetTemplate(string fileName, string className)
        {
            Helper.EnumTypes(fileName, t =>
            {
                if (t.FullName == className)
                {
                    var tb = new AspNetGenerator(t);
                    Console.WriteLine(tb.ToString());
                    return false;
                }
                return true;
            });
        }

        private static bool SearchClasses(string fileName, int argCount)
        {
			if (argCount == 2) {
				Helper.EnumTypes (fileName, t => {
					Console.WriteLine (t.FullName);
					return true;
				});
				return true;
			}
			return false;
        }

        private static void ShowTableList()
        {
            var g = new ModelsGenerator();
            foreach (var table in g.GetTableList())
            {
                Console.WriteLine(table);
            }
        }

        private static void GenerateModelFromDatabase(string tableName)
        {
            var g = new ModelsGenerator();
            Console.WriteLine(g.GenerateModelFromDatabase(tableName));
        }

        private static void ShowHelp()
        {
            string s = ResourceHelper.ReadToEnd(typeof(Program), "Readme.txt");
            Console.WriteLine(s);
        }
    }
}
