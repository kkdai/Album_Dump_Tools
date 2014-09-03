#PHP Albums Ouput Tools
It storage some webside dump helper for myself. Hope it help for you if any.


##4image PHP Dump Helper

It is a simple dump php to dump all photo, name, description from [4image](http://www.4homepages.de/). 

### How to use it.

Just put 4image_dump.php in your website which have [4image](http://www.4homepages.de/). Change the default configuration and run it.

You need update following default setting in 4image_dmp.php

    $USER_NAME 		  = "root";
    $PASSWORD 		  = "PASSWORD";
    $DATABASE_NAME 	  = "DB_NAME";
    $SOURCE_PATH      = "..\photo\data\media\\"; 
    // could be related path or absolute path.


### Output Result

- It will copy all photo from [4image](http://www.4homepages.de/) folder media\*.* to local.
- Will get all category name and description as follow:
    - FOLDER_NAME.TXT --> To store category name.
    - FOLDER_DESC.TXT --> To store category description.
-  For each photo it will output information as follow for photo ex:"TEST.jpg"
    -  "TEST.jpg.name.txt" --> To store photo name.  
    -  "TEST.jpg.desc.txt" --> To store photo description.      
   
     