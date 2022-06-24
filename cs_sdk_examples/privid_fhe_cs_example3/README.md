Privid Face C# Test Application 
===============================
Ver = 1.4.x

Purpose : To provide a Windows C# Desktop Commandlind Application to test Privid Face DLL. 

A. Prepare
----------	
	Set the Working Directory and keep all the test images and runtime DLLs accordingly. For example, to keep the working directory $(ProjectDir), the directory where the project file is saved. 
	In that case, update :
	1. Working Directory = $(ProjectDir)
	2. add pre-build commands 
      
      xcopy /E /Y  $(Pkgprivid_fhe_cs_example2)\test.src $(ProjectDir)
      xcopy /E /Y  $(Pkgprivid_fhe_cs_example2)\Results.src $(ProjectDir)
      xcopy  *.src *.cs
      xcopy /E /Y  $(Pkgprivid_fhe_cs_example2)\lib\privid_fhe.dll $(ProjectDir)
      xcopy /E /Y  $(Pkgprivid_fhe_cs_example2)\img\a1.png $(ProjectDir)
      xcopy /E /Y  $(Pkgprivid_fhe_cs_example2)\privid_app_setting.xml $(ProjectDir)
      xcopy /E /Y  $(Pkgprivid_fhe_cs)\lib\*.dll $(ProjectDir)

	3. To make the auto copy of correct runtime DLLs to the working folder,
		Add following lines to 'Post-build event' in Project Options->Build->Events
			xcopy /E /Y  $(Pkgprivid_fhe_cs)\lib\*.dll $(ProjectDir)
		Edit the project file line 
			From :
			<PackageReference Include="privid_fhe_cs_example2" Version="1.3.1"/>
			To :
			<PackageReference Include="privid_fhe_cs" Version="x.y.z" GeneratePathProperty="true" />
			<PackageReference Include="privid_fhe_cs_example2" Version="1.3.1" GeneratePathProperty="true" />

B. Test Procedure 
-----------------
Set the command line arguments for testing the respective APIs :  

    a. enroll  one image, set command line argument as 
	a1.png 0 enroll       

    b. predict one image, 
	a1.png 0 predict

    c. check image validity,
	a1.png 0 is_valid

    d. remove an enrolled entry 
	
	0c550dd1ea1b68e6a6c837 1 delete

	e. compare 
	<folder name with images to compare> <type of images>   compare <reference image>
	..\images\ "*.png"   compare IMG_3742.ppm.png

C. Release Notes (1.3.2)
----------------
1. Added support for returning Response structure instead of bool.
2. fixed uuid issue for first time enroll case.
3. support for cache flush added 
4. uuid and guid are stored after descrpting and delete is called after encrypting 
5. tested with privid_fhe_cs 1.9.2.
6. support for automatic DLL copy to working folder


C. Release Notes (1.4.1)
----------------
1. Tested 2.0.x privid_fhe_cs DLL
2. Test wrapper for document model 
3. Server details updated
