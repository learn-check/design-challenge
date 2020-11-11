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
    
    if (r){
      document.getElementById("button").action;
    }
    else{
      alert("Dat is jammer tot de volgende keer.")
      return r;
    }
  }

  // dit is ervoor om te contact op te nemen met een van onze medewerkers. door middel van email.
  var emails = [  "mailto:404368@student.fontys.nl", // Art 0
                  "mailto:462404@student.fontys.nl", // Ruben 1
                  "mailto:367898@student.fontys.nl", // Robbie 2
                  "mailto:463253@student.fontys.nl", // ME 3
                  "mailto:455150@student.fontys.nl" ]; // Mathijs 4

  function openContact(index)
  {
    var windows = window.open(emails[index]);
  }
