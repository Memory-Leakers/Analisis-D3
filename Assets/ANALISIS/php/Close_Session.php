<?php		
		$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
		//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

		if ($mysqli->connect_error) {
			die("Connection failed: " . $mysqli->connect_error);
		}
		
		if ($_SERVER["REQUEST_METHOD"] == "POST")
		{
			$id = $_POST['id'];
			$end_datetime = $_POST['end_datetime'];
				
			$sql = "UPDATE Sessions SET end_datetime = '$end_datetime' WHERE Sessions.Id = '$id';";
				
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