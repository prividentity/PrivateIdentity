
build:
	dotnet build --configuration Debug

clean:
	dotnet clean 
	rm -rf obj
	rm -rf libprivid_fhe.so
	rm -rf *0.bmp
	rm -rf ./bin
	rm -rf published
	

run: 
	make is_valid 
	make enroll 
	make predict


is_valid:
	dotnet ./bin/Debug/net6.0/privid_fhe_cs_example3.dll  a1.png 0 $@
 
enroll:
	dotnet ./bin/Debug/net6.0/privid_fhe_cs_example3.dll  a1.png 0 $@
 
predict:
	dotnet ./bin/Debug/net6.0/privid_fhe_cs_example3.dll  a1.png 0 $@
	
tar: clean
		tar cvf privid_fhe_cs_example3.tar *



