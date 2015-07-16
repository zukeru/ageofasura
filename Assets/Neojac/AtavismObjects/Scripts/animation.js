#pragma strict


function Update () {

if (Input.GetKey("1"))
    {
 
    GetComponent.<Animation>().Play("open");
    }

if (Input.GetKey("2"))
    {
    GetComponent.<Animation>().Play("close");
    }

if (Input.GetKey("3"))
    {
    GetComponent.<Animation>().Play("vibrate");
    } 

}