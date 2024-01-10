<?php		
	$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
	//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

	if ($mysqli->connect_error) {
		die("Connection failed: " . $mysqli->connect_error);
	}
		
	if ($_SERVER["REQUEST_METHOD"] == "GET")
	{		
		$sql = "SELECT * FROM Sessions;";
			
			
		$result =  $mysqli->query($sql);
		if ($result) 
		{
			$num_rows = $result->num_rows;
			if ($num_rows > 0)
			{
				$data = array();

				while ($row = $result->fetch_assoc()) 
				{
					$data[] = array(
						"_session_id" => $row["Id"],
						"_start_datetime" => $row["start_datetime"],
						"_end_datetime" => $row["end_datetime"],
						"events" => array()
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