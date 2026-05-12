
Project for reading the 2box/drumit 3 or 5 .dsnd files and extract the samples from it,
and generating .dsnd from samples.

<br />
<br />

**Commandline arguments and examples**  
<br />

*Main operation:*  
-e does the export, then the first argument on the commandline is the name of the dsnd file or wildcard,   
-g generates dsnd file(s)  
<br />
<br />
*Other options:*  
-t <n> where n is number of max threads, minimal 2 when present  
-r recursive  
-o overwrite existing files  
-p "export directory" (mandantory for export)  

<br />
<br />

*Examples:*  
* Export dsnd to seperate wav's:  
Dsnd.CLI.exe s:\\snares\\dd_SNRE_571_Gaynor.dsnd" -e  -p "s:\\export_directory"  


* Generate dsnd:  
Dsnd.CLI.exe -g "s:\\import_directory"


