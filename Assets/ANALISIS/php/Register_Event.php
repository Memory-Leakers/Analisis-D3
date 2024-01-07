<?php		
		$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
		//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

		if ($mysqli->connect_error) {
			die("Connection failed: " . $mysqli->connect_error);
		}
		
		if ($_SERVER["REQUEST_METHOD"] == "POST")
		{
			$Type = $_POST['Type'];
			$Level = $_POST['Level'];
			$Position_X = $_POST['Position_X'];
			$Position_Y = $_POST['Position_Y'];
			$Position_Z = $_POST['Position_Z'];
			$Session_id = $_Post['Session_id'];
			$Date = $_POST['Date'];
			$Step = $_POST['Step']
				
			$sql = "INSERT INTO Events (Type, Level, Position_X, Position_Y, Position_Z, Session_id, date, step) VALUES ('$Type', '$Level', '$Position_X', '$Position_Y', '$Position_Z', '$Session_id', '$Date', '$Step');";
				
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