<?php		
		$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
		//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

		if ($mysqli->connect_error) {
			die("Connection failed: " . $mysqli->connect_error);
		}
		
		if ($_SERVER["REQUEST_METHOD"] == "GET")
		{		
			$sql = "SELECT * FROM Sessions;";
				
			if ($mysqli->query($sql) === TRUE) 
			{
				if ($mysqli->num_rows > 0) 
				{
					// Fetch each row and echo the data
					//while ($row = $mysqli->fetch_assoc()) 
					//{
					//	echo "Session ID: " . $row["Id"] . ", Start: " . $row["start_datetime"] . ", End: " . $row["end_datetime"] . "<br>";
					//}
					$row = $mysqli->fetch_assoc();
					
						echo "Session ID: " . $row["Id"] . ", Start: " . $row["start_datetime"] . ", End: " . $row["end_datetime"] . "<br>";
					
				} 
				else 
				{
					echo "No rows found";
				}	
			} 
			else 
			{
				echo "Error: " . $sql . "<br>" . $mysqli->error;
			}
		}
	?>