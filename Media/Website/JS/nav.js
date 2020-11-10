function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
  }
  // dit opent de side nav
  function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
  }
  // dit sluit de sidenav
  function modal() {
    modal()
  }

  function areUSure(){
    var r = confirm("weet je het zeker dat je deze reis wilt?");
    var home = document.getElementsByName("../home.html")
    if (r == true){
      document.getElementById("button").action;
    }
    else{
      alert("dat is jamer tot de volgende keer")
    }
  }
  // dit is een jav functie die ik heb geschreven zodat  de klant er zeker van is dat hij de goede reis heeft gekozen.