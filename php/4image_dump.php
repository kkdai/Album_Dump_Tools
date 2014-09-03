<?php

$USER_NAME 		  = "root";
$PASSWORD 		  = "PASSWORD";
$DATABASE_NAME 	= "DB_NAME";
$SOURCE_PATH    = "..\photo\data\media\\"; 
// could be related path or absolute path.

///////////////////
mysql_connect("localhost", $USER_NAME, $PASSWORD) or die("Could not connect: " . mysql_error());
mysql_select_db($DATABASE_NAME);
 
$cat_result = mysql_query("SELECT cat_id, cat_parent_id, cat_name, cat_description  FROM 4images_categories");

// Categories query 
while ($cat_row = mysql_fetch_array($cat_result, MYSQL_NUM)) {
	$cat_id = $cat_row[0];
	$cat_parent_id = $cat_row[1];
	$cat_name = $cat_row[2];
	$cat_desc = $cat_row[3];

  if ($cat_id != 0) {
  	//it is leaf catgories
  	echo "cat_id: ".$cat_id."<br>";
  	echo "cat_parent_id: ".$cat_parent_id."<br>";
  	echo "cate name: ".$cat_name."<br>";
  	echo "cate desc: ".$cat_desc."<br>";
  	echo "starting show images<br><hr>";

  	//create folder
  	$make_path = "download\\".$cat_id."\\";
    if (!is_dir($make_path)) {
      mkdir($make_path,0777,TRUE);
    }

  	//write cate name/desc into FOLDER.txt
  	file_put_contents($make_path. "FOLDER_NAME.TXT", $cat_name);
  	file_put_contents($make_path. "FOLDER_DESC.TXT", $cat_desc);

  	//starting to query photo.
  	$img_result = mysql_query("SELECT image_name, image_description, image_media_file FROM 4images_images WHERE cat_id=".$cat_id);
  	while ($img_row = mysql_fetch_array($img_result, MYSQL_NUM)) {
  		$img_name = $img_row[0];
  		$img_desc = $img_row[1];
  		$img_filename = $img_row[2];
      //moving photo from source path to target path.
  		copy( $SOURCE_PATH .$cat_id."\\".$img_filename, $make_path.$img_filename);
  		//write all related data in *.txt
  		$output_name_file = $make_path.$img_filename.".name.txt";
  		$output_desc_file = $make_path.$img_filename.".desc.txt";
  		file_put_contents($output_name_file, $img_name);
  		echo "write file name: ". file_get_contents($output_name_file) . "<br>";
  		file_put_contents($output_desc_file, $img_desc);
  		echo "write file desc: ". file_get_contents($output_desc_file) . "<br>";
  	}
  }
  echo "<hr><p>";
}
 
mysql_free_result($cat_result); // clean memory
mysql_free_result($img_result); // clean memory
echo "<br><h1>Done.....</h1><br>";
?>