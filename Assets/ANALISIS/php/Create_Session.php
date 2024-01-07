<?php		
		$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
		//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

		if ($mysqli->connect_error) {
			die("Connection failed: " . $mysqli->connect_error);
		}
		
		if ($_SERVER["REQUEST_METHOD"] == "POST")
		{
			$start_datetime = $_POST['start_datetime'];
				
			$sql = "INSERT INTO Sessions (start_datetime) VALUES ('$start_datetime');";
				
			if ($mysqli->query($sql) === TRUE) 
			{
				echo $mysqli->insert_id;	
			} 
			else 
			{
				echo "Error: " . $sql . "<br>" . $mysqli->error;
			}
		}
	?>