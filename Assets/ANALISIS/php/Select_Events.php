<?php		
	$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
	//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

	if ($mysqli->connect_error) {
		die("Connection failed: " . $mysqli->connect_error);
	}
		
	if ($_SERVER["REQUEST_METHOD"] == "GET")
	{		
		$Ids = $_POST["Ids"];
		
		$sql = "SELECT * FROM Events WHERE Session_id IN ('$Ids')";
			
		$result =  $mysqli->query($sql);
		if ($result) 
		{
			$num_rows = $result->num_rows;
			if ($num_rows > 0)
			{
				$data = array();
				// Fetch each row and echo the data
				while ($row = $result->fetch_assoc()) 
				{
					$data[] = array(
						"Id" => $row["Id"],
						"Type" => $row["Type"],
						"Level" => $row["Level"],
						"Position_X" => $row["Position_X"],
						"_session_id" => $row["Id"],
						"_start_datetime" => $row["start_datetime"],
						"_end_datetime" => $row["end_datetime"]
					);
				}
				echo json_encode($data);
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