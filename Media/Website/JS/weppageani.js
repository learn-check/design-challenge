
const tl = gsap.timeline({ defaults: { ease: "power1.out" } });
// hier maak ik de const timeline aan. en zet ik de default aimatie naa "power2.out"

tl.to(".slider", { y: "-100%", duration: 1});
// dit is ervoor zodat de roze slide van onder naar boven gaat.
tl.to(".intro", { y: "-100%", duration: 0.5 }, "-=1");
// dit is ervoor om de pagina weer tevoorschijn te krijgen.





