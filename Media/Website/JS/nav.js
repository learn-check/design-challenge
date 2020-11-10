function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
  }
  // dit opent de side nav
  function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
  }
  // dit sluit de sidenav
 

  function areUSure(){
    var r = confirm("weet je het zeker dat je deze reis wilt?");
    
    if (r == true){
      document.getElementById("button").action;
    }
    else{
      alert("dat is jamer tot de volgende keer")
      return false;
    }
  }
  // dit is een jav functie die ik heb geschreven zodat  de klant er zeker van is dat hij de goede reis heeft gekozen.