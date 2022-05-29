/*---------------------------------------------------------------------------------------
  Damian Jankov
  Siemens Healthcare- zadanie student,pohovor
  30.05.2022

  This code takes an input of form of path to folder and puts its context into .json file
  that also needs to be provided.

  Code dynamically, with the usage of recursion makes an object of Folder containing
  all subfolders and files, as objets.

  Code can also take input as .json and make it into an object, that can be stored as 
  a json again.

  Code was tested with the max depth of folders of 7, the CLI client part of the code
  should also be able to work with wrong inputs such as random words or so.

  Code assumes .json is already written in correct format, and since this is just a demo
  everyting is public.
 -----------------------------------------------------------------------------------------*/



using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Projekt1
{

    class Program
    {
        static void Main(string[] args)
        {
                
        
        Tools_zadanie.app();
         

        }

    }


    static class Tools_zadanie
    {

  /*-----------------------------------------------------------------------------------------*/

/*  This part of code is responsible for CLI communication with user, using recursion
    all scenarios with wrong inputs or so shoud be considered covered  
 */


        /*  Provide valid input,if not valid then recursion till valid, if "exit" then exiting cli */
        public static string enter_valid_input()
        {

            Console.WriteLine("\n\nPlease provide a folder or a json with relevant folder information:");
            string input = Console.ReadLine();
           if (input.ToLower().Equals("exit"))
            {
                
                Environment.Exit(0);
            }
            string path = @"" + input;
            if (File.Exists(path))
            {
                if (!path.Contains(".json"))
                {
                    Console.WriteLine("Please enter valid input!");
                    enter_valid_input();
                }
              
            }else if (!Directory.Exists(path))
            {
                Console.WriteLine("Please enter valid input!");
                enter_valid_input();
            }
            return path;
        }

        public static void app( )
        {
            string valid_path = Tools_zadanie.enter_valid_input();


            if (Directory.Exists(valid_path))
            {
                is_folder(valid_path);
            }

            /* Already asumes right .json format because of enter_valid_input() */
            if (File.Exists(valid_path) && new FileInfo(valid_path).Length != 0)
            {
               
                is_file(valid_path);
            }else //json must hold some information before it is passed
            {
                Console.WriteLine("Provided json has no contents!");
                app();
            }
            

        }


      


        /*  If it is a folder, makes a "main" folder and sends it with path to
         tree_recursion (objects maker)  
         */
        public static void is_folder(string path)
        {
            Folder hlavny = new Folder();
          
            Tools_zadanie.tree_recursion(path, hlavny);
            unique_extensions_lister(hlavny);

            save_to_json(hlavny);
     


            app();
        }

    

        /*  If its  file,then its already considered that it is of a .json 
         *  with some correct contents in,before passed in */
        public static void is_file(string path)
        {


            /* Asumes valid json input beforehand because of enter_valid_input*/
            /* Must be .json, must hold some information, correct form of informations is not covered!*/
            Folder p = null;
            DataSerializer dataSerializer = new DataSerializer();
            p = dataSerializer.JsonDeserialize(typeof(Folder), path) as Folder;
            unique_extensions_lister(p);
            save_to_json(p);


            app();

        }

        /* Saves to json an Folder object */
        public static void save_to_json(Folder hlavny)
        {
            Console.WriteLine("Save to json?");
            string input = Console.ReadLine();
            if (input.Equals("y") || input.Equals("yes"))
            {
                provide_json(hlavny);

            }
            else if (input.Equals("no") || input.Equals('n'))
            {
                app();
            }
            else if (input.ToLower().Equals("exit"))
            {

                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Please provide correct answer yes/no");
                save_to_json(hlavny);
            }


        }




        public static void provide_json(Folder hlavny)
        {
            Console.WriteLine("Please provide the JSON file location");
            string json_loc = Console.ReadLine();
            if (File.Exists(json_loc) && json_loc.Contains(".json"))
            {
                DataSerializer dataSerializer = new DataSerializer();
                Folder p = null;

                dataSerializer.JsonSerialize(hlavny, json_loc);

            }
            else if (json_loc.ToLower().Equals("exit"))
            {

                Environment.Exit(0);
            }
            else
            {
                provide_json(hlavny);
            }

        }





        /* Extension finder, finds and prints unique extension of folder */
        public static void unique_extensions_lister(Folder target)
        {
            List<string> found_uni = new List<string>();
            file_ext_finder(target, found_uni);

            List<string> extensions = new List<string>();


            foreach (string x in found_uni)
            {

                if (x.Contains('.')) //some error with Makefile
                {
                    if (!extensions.Contains(x.Substring(x.LastIndexOf('.'))))
                    {
                        extensions.Add(x.Substring(x.LastIndexOf('.')));
                    }
                }
            }
            
            Console.Write("Extensions found in folder:");
            foreach (string x in extensions)
            {
                Console.Write(x + " ");
            }

            Console.WriteLine();
        }



        public static void file_ext_finder(Folder target, List<string> list)
        {

            foreach (string x in target.subory)
            {
                if (!list.Contains(x))
                {
                    list.Add(x);
                }

            }

            foreach (Folder a in target.priecinky)
            {
                file_ext_finder(a, list);
            }


        }

 /*-----------------------------------------------------------------------------------------*/






 /*-----------------------------------------------------------------------------------------*/

        /* ***This part of code makes objects from a folder given folderpath*** */


   

        public static Folder tree_recursion(string path, Folder curr_folder)
        {
            curr_folder.Name = get_name(path); //get name trims the string
            /*Contents of current folder in filepaths*/
            string[] filePaths = combine_folder_file_paths(path);



            for (int i = 0; i < filePaths.Length; i++)
            {
                if (File.Exists(filePaths[i])) 
                {
                    /*If File then it is being added to file list in current folder*/
                    curr_folder.subory.Add(get_name(filePaths[i]));
                }
                else
                {
                    /*If not file then folder and it is being added to folder list of current folder*/

                    Folder tmp = new Folder();                  
                    tmp.Name = get_name(filePaths[i]);              
                    curr_folder.priecinky.Add(tmp);
                }
            }


            /*
              If contains any folders then recuresively call tree_recursion for every single one of them
             */
            bool isEmpty = !curr_folder.priecinky.Any();
            if (!isEmpty)
            {
                for (int i = 0; i < curr_folder.priecinky.Count; i++)
                {                
                    curr_folder.priecinky[i] = tree_recursion(path +'\\'+ curr_folder.priecinky[i].Name, curr_folder.priecinky[i]);
                }
            }
            return curr_folder;
        }
        public static string get_name(string path_of_folder)
        {
            /* Makes a name from folderpath*/
            return path_of_folder.Substring(path_of_folder.LastIndexOf('\\') + 1);
        }
        public static string[] combine_folder_file_paths(string path)
        {
            /* Combining list of folder and file paths into one array*/
            List<string> list = new List<string>();
            string[] filePaths = Directory.GetFiles(path);
            string[] folderDirectories = Directory.GetDirectories(path);

            foreach(string item1 in filePaths)
            {    
                list.Add(item1);
            }
            
            foreach (string item2 in folderDirectories)
            {            
                list.Add(item2);
            }

            string[] combined = list.ToArray();
            Array.Sort(combined);

            return combined;
        }

  /*-----------------------------------------------------------------------------------------*/

 }





    /*-----------------------------------------------------------------------------------------*/

    /*  ***This part of code serializes and deserialized from and to JSON format*** */
    class DataSerializer
    {
        public void JsonSerialize(object data,string filePath)
        {
            JsonSerializer jsonSerializer= new JsonSerializer();
            if (File.Exists(filePath)) File.Delete(filePath);

           
            StreamWriter sw = new StreamWriter(filePath);

            JsonWriter jsonWriter = new JsonTextWriter(sw);

            jsonSerializer.Serialize(jsonWriter, data);


            jsonWriter.Close();
            sw.Close();
        }

        public  object JsonDeserialize(Type dataType,string filePath)
        {
            JObject obj = null;
            JsonSerializer jsonSerializer= new JsonSerializer();
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                JsonReader jsonReader= new JsonTextReader(sr);
                obj = jsonSerializer.Deserialize(jsonReader) as JObject;
                jsonReader.Close();
                sr.Close();
            }
            return obj.ToObject(dataType);
        }

    }
    /*-----------------------------------------------------------------------------------------*/
}