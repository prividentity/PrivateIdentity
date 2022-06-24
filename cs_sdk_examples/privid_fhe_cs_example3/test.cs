using privid_fhe_cs;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

public class privid_fhe_tst
{

    static public string folder_name_local_storage;
    static public string server_url;
    static public string api_key;
    static public int k_factor;

    private static void clearFolder(string FolderName)
    {
        if (Directory.Exists(FolderName))
            Directory.Delete(FolderName, true);
    }


    private static void settings_read(int option)
    {
        XmlDataDocument xmldoc = new XmlDataDocument();
        XmlNodeList xmlnode;

        FileStream fs = new FileStream("privid_app_setting.xml", FileMode.Open, FileAccess.Read);
        xmldoc.Load(fs);

        folder_name_local_storage = xmldoc.GetElementsByTagName("folder_name_local_storage")[0].InnerText;
        server_url = xmldoc.GetElementsByTagName("server_url")[0].InnerText;
        api_key = xmldoc.GetElementsByTagName("api_key")[0].InnerText;
        k_factor = Int32.Parse(xmldoc.GetElementsByTagName("k_factor")[0].InnerText);

        if (option > 2)
        {
            Console.WriteLine("Settings:");
            Console.WriteLine("     [Settings ] server_url =                {0}", xmldoc.GetElementsByTagName("server_url")[0].InnerText);
            Console.WriteLine("     [Settings ] folder_name_local_storage = {0}", xmldoc.GetElementsByTagName("folder_name_local_storage")[0].InnerText);
            Console.WriteLine("     [Settings ] api_key                   = {0}", xmldoc.GetElementsByTagName("api_key")[0].InnerText);
            Console.WriteLine("     [Settings ] k_factor                  = {0}", xmldoc.GetElementsByTagName("k_factor")[0].InnerText);
        }
    }

    private static void settings_print()
    {
        Console.WriteLine("     [Settings ] server_url =                {0}", server_url);
        Console.WriteLine("     [Settings ] folder_name_local_storage = {0}", folder_name_local_storage);
        Console.WriteLine("     [Settings ] api_key                   = {0}", api_key);
        Console.WriteLine("     [Settings ] k_factor                  = {0}", k_factor);

    }

    private static bool IsPathDirectory(string path)
    {
        if (path == null) throw new ArgumentNullException("path");
        path = path.Trim();

        if (Directory.Exists(path))
            return true;

        if (File.Exists(path))
            return false;

        // neither file nor directory exists. guess intention

        // if has trailing slash then it's a directory
        if (new[] { "\\", "/" }.Any(x => path.EndsWith(x)))
            return true; // ends with slash

        // if has extension then its a file; directory otherwise
        return string.IsNullOrWhiteSpace(Path.GetExtension(path));
    }

    public static string WriteFromObject(object obj)
    {
        byte[] json;
        //Create a stream to serialize the object to.  
        using (MemoryStream ms = new MemoryStream())
        {
            // Serializer the object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(ms, obj);
            json = ms.ToArray();
            ms.Close();
        }
        return Encoding.UTF8.GetString(json, 0, json.Length);

    }

    // Deserialize a JSON stream to object.  
    public static T ReadToObject<T>(string json) where T : class, new()
    {
        T deserializedObject = new T();
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {

            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedObject.GetType());
            deserializedObject = ser.ReadObject(ms) as T;
            ms.Close();
        }
        return deserializedObject;
    }

    public static int Main(string[] args)
    {
        Console.Write("\nTest PrividModule with C# interface. 1236\n");
        Console.Write("\n C++ DLL version {0}, C# Wrapper DLL version {1} : ", privid_fhe_face.get_version(), privid_fhe_face.get_cs_dll_version());
        int option = 0;
        bool ret = true;
        string run_api = "none";
        bool proceed = true;

        Dictionary<int, t_privid_results_is_valid> InfoIsValid = new Dictionary<int, t_privid_results_is_valid>();
        Dictionary<string, Response> InfoEnroll = new Dictionary<string, Response>();
        Dictionary<string, Response> InfoPredict = new Dictionary<string, Response>();

        if (args.Length != 0)
        {
            switch (args[0])
            {
                case "settings":
                    settings_read(1);
                    proceed = false;
                    break;
                default:

                    break;
            }
            if (proceed == false)
            {
                return -1;
            }
        }
        if (args.Length == 3)
        {
            try
            {
                option = int.Parse(args[1]);
            }
            catch { option = 0; }

            run_api = args[2];
        }
        else if (args.Length == 4)
        {
            option = 0;
            run_api = args[2];
        }
        else
        {
            System.Console.WriteLine("INFO  : No valid arguments passed ");
            System.Console.WriteLine("USAGE : {0} <image_file> <debug_level> <test_id>   OR ", Process.GetCurrentProcess().ProcessName);
            System.Console.WriteLine("USAGE : {0} <image_dir> <file_type_filter> <test_id>    OR", Process.GetCurrentProcess().ProcessName);
            System.Console.WriteLine("USAGE : {0} <image_dir> <file_type_filter> compare  <ref image> ", Process.GetCurrentProcess().ProcessName);

            return 1;
        }

        string out_filder = "./out/";
        if(!Directory.Exists(out_filder))
        {
            out_filder = "./";
        }

        if (run_api != "delete" && run_api != "flush")
        {
            if (File.Exists(args[0]) == false && IsPathDirectory(args[0]) != true)
            {
                Console.WriteLine("Image file {0} does not exist.", args[0]);
                return -1;
            }

            if (run_api == "compare")
            {
                if (IsPathDirectory(args[0]) == true)
                {
                    if (File.Exists(args[3]) == false)
                    {
                        Console.WriteLine("Image file B {0} does not exist.", args[3]);
                        return -1;
                    }
                }

                //if(option > 0) 
                Console.Write("{0} files {1} & {2} : ", run_api, args[0], args[1]);
            }
            else
            {
                //if (option > 0) 
                Console.Write("{0} with {1} : ", run_api, args[0]);
            }
        }

        //Console.WriteLine();

        settings_read(option);
        if (option > 0)
            Console.WriteLine(" server_url = {0}  api_key = {1}", server_url, api_key);

        privid_fhe_face privid_fhe_face1 = new privid_fhe_face(server_url, folder_name_local_storage, api_key, option);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        switch (run_api)
        {
            case "is_valid":
                if (!IsPathDirectory(args[0]))
                {
                    t_privid_results_is_valid valid_result = privid_fhe_face1.is_valid(Image.Load(args[0]), (int)privid_fhe_face.nContextEnum.FacePhotoPredictRGB);
                    valid_result.OutputResults(args[0], out_filder + Path.GetFileName(args[0]) + valid_result.result + ".bmp");
                }
                else
                {
                    int idx = 0;
                    string[] files = Directory.GetFiles(args[0], args[1], SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        t_privid_results_is_valid valid_result = privid_fhe_face1.is_valid(Image.Load(file), (int)privid_fhe_face.nContextEnum.FacePhotoPredictRGB);
                        valid_result.OutputResults(args[0], out_filder + Path.GetFileName(file) + valid_result.result + ".bmp");

                        try
                        {
                            InfoIsValid.Add(idx++, valid_result);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                break;
            case "is_valid_barcode":
                if (!IsPathDirectory(args[0]))
                {
                    t_privid_results_is_valid valid_result = privid_fhe_face1.is_valid(Image.Load(args[0]), (int)privid_fhe_face.nContextEnum.IdBarcodeRGB);
                    valid_result.OutputResults(args[0], out_filder + Path.GetFileName(args[0]) + valid_result.result + ".bmp");
                }
                else
                {
                    int idx = 0;
                    string[] files = Directory.GetFiles(args[0], args[1], SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        t_privid_results_is_valid valid_result = privid_fhe_face1.is_valid(Image.Load(file), (int)privid_fhe_face.nContextEnum.IdBarcodeRGB);
                        valid_result.OutputResults(args[0], out_filder + Path.GetFileName(file) + valid_result.result + ".bmp");

                        try
                        {
                            InfoIsValid.Add(idx++, valid_result);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                break;

            case "is_valid_face_id":
                if (!IsPathDirectory(args[0]))
                {
                    t_privid_results_is_valid document_result = privid_fhe_face1.document_check(Image.Load(args[0]), (int)privid_fhe_face.nContextEnum.IdPhotoRGB);
                    document_result.OutputResults(args[0], out_filder + Path.GetFileName(args[0]) + document_result.result + ".bmp");
                }
                else
                {
                    int idx = 0;
                    string[] files = Directory.GetFiles(args[0], args[1], SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        t_privid_results_is_valid document_result = privid_fhe_face1.document_check(Image.Load(file), (int)privid_fhe_face.nContextEnum.IdPhotoRGB);
                        document_result.OutputResults(args[0], out_filder + Path.GetFileName(file) + document_result.result + ".bmp");

                        try
                        {
                            InfoIsValid.Add(idx++, document_result);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                break;
            case "enroll":
                if (!IsPathDirectory(args[0]))
                {
                    Response enroll_result = privid_fhe_face1.enroll(Image.Load(args[0]));
                    if (option > 0) Console.WriteLine("     RESULT  : enroll_result for Image {0} is = {1}", args[0], enroll_result.get_json());
                    if (enroll_result.PI_list.Count > 0)
                        Console.WriteLine(" uuid = {0}", enroll_result.PI_list[0].uuid);
                    else
                        Console.WriteLine(" no valid uuid");
                }
                else
                {
                    string[] files = Directory.GetFiles(args[0], args[1], SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        Response enroll_result = privid_fhe_face1.enroll(Image.Load(file));
                        Console.Write(" {0,-28} :", file);
                        if (enroll_result.PI_list.Count > 0)
                        {
                            Console.WriteLine(" uuid = {0,16} {1}", enroll_result.PI_list[0].uuid, enroll_result.get_json());
                            try
                            {
                                InfoEnroll.Add(enroll_result.PI_list[0].uuid, enroll_result);
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex.ToString());
                            }

                        }
                        else
                            Console.WriteLine(" {0,16} {1}", "     ***NOT ENROLLED***    ", enroll_result.get_json());
                    }
                }

                break;

            case "predict":
                if (!IsPathDirectory(args[0]))
                {


                    Response predict_result = privid_fhe_face1.predict(Image.Load(args[0]), k_factor);
                    if (predict_result.PI_list.Count > 0)
                    {
                        Console.Write(" uuid = {0}", predict_result.PI_list[0].uuid);
                        try
                        {
                            InfoPredict.Add(predict_result.PI_list[0].uuid, predict_result);
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex.ToString());
                        }
                    }
                    else
                        Console.WriteLine(" no valid uuid");

                    if (option > 0) Console.WriteLine("     RESULT  : predict_result for Image {0} is = {1}", args[0], predict_result.get_json());

                }
                else
                {
                    int maxLength = 164;
                    string[] files = Directory.GetFiles(args[0], args[1], SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        Response predict_result = privid_fhe_face1.predict(Image.Load(file), k_factor);
                        Console.Write(" {0,-28} :", file);
                        if (predict_result.PI_list.Count > 0)
                            Console.WriteLine(" uuid = {0,16} {1}...", predict_result.PI_list[0].uuid, predict_result.get_json().Substring(0, maxLength));
                        else
                            Console.WriteLine(" {0,16} {1}", "   ***COULDN'T PREDICT***  ", predict_result.get_json());
                    }
                }
                break;
            case "delete":
                if (File.Exists(args[0]))
                {
                    Response predict_result = privid_fhe_face1.predict(Image.Load(args[0]), k_factor);
                    if (predict_result.PI_list.Count > 0)
                    {
                        Console.WriteLine(" uuid = {0}", predict_result.PI_list[0].uuid);
                        try
                        {
                            string delete_result = privid_fhe_face1.delete(predict_result.PI_list[0].uuid);
                            if (option > 0) Console.WriteLine("     RESULT  : delete_result for file {0} UUID = {1} is = {2}", args[0], predict_result.PI_list[0].uuid, delete_result);
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex.ToString());
                        }
                    }
                }
                else
                {
                    string delete_result = privid_fhe_face1.delete(args[0]);
                    if (option > 0) Console.WriteLine("     RESULT  : delete_result for UUID {0} is = {1}", args[0], delete_result);
                }
                break;
            case "compare":
                if (!IsPathDirectory(args[0]))
                {

                    t_privid_results_compare compare_result = privid_fhe_face1.compare_files(Image.Load(args[0]), Image.Load(args[1]), option);
                    //int compare_result = privid_fhe_face1.compare_files(Image.FromFile(args[0]), Image.FromFile(args[0]), 2);
                    bool proceed_flag = true;
                    if (compare_result.valid_flag_a != 0)
                    {
                        Console.WriteLine(" image_a is invalid");
                        proceed_flag = false;
                    }
                    if (compare_result.valid_flag_b != 0)
                    {
                        Console.WriteLine(" image_b is invalid");
                        proceed_flag = false;
                    }
                    if (proceed_flag)
                    {
                        if (option > 0) Console.WriteLine("{0}", compare_result.ToString(args[0], args[1]));
                        else Console.WriteLine("Images {0} & {1} are {2}", args[0], args[1], compare_result.result == 0 ? "Same" : "Different");
                    }
                }
                else
                {
                    string[] files = Directory.GetFiles(args[0], args[1], SearchOption.TopDirectoryOnly);
                    string reffile = args[3];
                    int countSame = 0;
                    int countTotal = 0;
                    foreach (string file in files)
                    {
                        countTotal++;
                        t_privid_results_compare compare_result = privid_fhe_face1.compare_files(Image.Load(file), Image.Load(reffile), option);
                        //int compare_result = privid_fhe_face1.compare_files(Image.FromFile(args[0]), Image.FromFile(args[0]), 2);
                        bool proceed_flag = true;
                        if (compare_result.valid_flag_a != 0)
                        {
                            Console.WriteLine("         {0, -32} is invalid", file);
                            proceed_flag = false;
                        }
                        if (compare_result.valid_flag_b != 0)
                        {
                            Console.WriteLine("         *****reffile {0, -32} is invalid", reffile);
                            proceed_flag = false;
                        }
                        if (proceed_flag)
                        {
                            string s = compare_result.ToString(reffile, file);
                            if (compare_result.result == 0)
                            {
                                countSame++;
                                Console.WriteLine("{0}", s);
                            }
                        }
                    }
                    Console.WriteLine("Compare {0} Same files outof {1}", countSame, countTotal);
                }
                break;
            case "flush":
                privid_fhe_face1.flush_cache(args[0]);
                break;
            default:

                Console.WriteLine("WARNING  : invalid api argument 3 = {0}, supported strings are is_valid/enroll/predict (case sensitive)", args[2]);

                break;
        }
        stopwatch.Stop();
        var elapsed_time = stopwatch.ElapsedMilliseconds;
        if (option > 0) Console.WriteLine("elapsed_time = {0} mSec", elapsed_time);

        return 0;
    }

}




