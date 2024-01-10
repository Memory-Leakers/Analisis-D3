<?php		
	$mysqli = new mysqli("localhost", "robertri", "qJabpNb3gG8L", "robertri");
	//$mysqli = new mysqli("localhost", "zhidac", "qJcPK4e6DVJj", "zhidac");

	if ($mysqli->connect_error) {
		die("Connection failed: " . $mysqli->connect_error);
	}
		
	if ($_SERVER["REQUEST_METHOD"] == "POST")
	{		
		$Ids = $_POST["Ids"];
		
		$sql = "SELECT * FROM Events WHERE Session_id IN ('$Ids');";
		$sql = str_replace("'", "", $sql);
		
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
						"Id" => $row["Id"],
						"Type" => $row["Type"],
						"Level" => $row["Level"],
						"Position_X" => $row["Position_X"],
						"Position_Y" => $row["Position_Y"],
						"Position_Z" => $row["Position_Z"],
						"Session_id" => $row["Session_id"],
						"date" => $row["date"],
						"step" => $row["step"]
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


