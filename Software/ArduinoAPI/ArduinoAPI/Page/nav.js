function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
  }
  // dit opent de side nav
  function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
  }
  // dit sluit de sidenav
 

  function areUSure(){
    var r = confirm("Weet u het zeker dat je deze reis wilt?");
    
    if (r == true){
      document.getElementById("button").action;
    }
    else{
      alert("Dat is jammer tot de volgende keer.")
      return false;
    }
  }
  // dit is een jav functie die ik heb geschreven zodat 
  // de klant er zeker van is dat hij de goede reis heeft gekozen.
  function contactAr(){
    var myWindow = window.open("mailto:404368@student.fontys.nl");
    
    
  }
  function contactRu(){
    var myWindow = window.open("mailto:462404@student.fontys.nl");
    
    
  }
  function contactRo(){
    var myWindow = window.open("mailto:367898@student.fontys.nl");
    
    
  }
  function contactBo(){
    var myWindow = window.open("mailto:463253@student.fontys.nl");
    
    
  }
  function contactMa(){
    var myWindow = window.open("mailto:455150@student.fontys.nl");
    
    
  }
  // dit is ervoor om te contact op te nemen met een van onze medewerkers. door middel van email.