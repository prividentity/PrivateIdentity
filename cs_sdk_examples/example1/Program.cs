using privid_fhe_cs;
using SixLabors.ImageSharp;

string infilename = "a1.png";
Console.WriteLine("Test App");
privid_fhe_face privid_fhe_face1 = new privid_fhe_face("https://priv.id/node/", "privid_local_storage1", "00000000000000001962", 0);

t_privid_results_is_valid valid_result = privid_fhe_face1.is_valid(Image.Load(infilename), (int)privid_fhe_face.nContextEnum.FacePhotoPredictRGB);
valid_result.OutputResults(infilename, "a1_cropped.bmp");
