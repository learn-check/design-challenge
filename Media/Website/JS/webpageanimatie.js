const tl = gsap.timeline({ defaults: { ease: "power1.out" } });

tl.to(".text", { y: "0%", duration: 1, stagger: 0.25 });
// dit is zodat de grote witte text in het zwarte scherm met een 
//stagger naar voren komt. Dit betekend dus is stages. 
tl.to(".slider", { y: "-100%", duration: 1.5, delay: 0.5 });
// dit is ervoor zodat de roze slide van onder naar boven gaat.
tl.to(".intro", { y: "-100%", duration: 1 }, "-=1");
// dit is ervoor om de pagina weer tevoorschijn te krijgen.
tl.fromTo("#nav", { opacity: 0 }, { opacity: 1, duration: 1 });
//dit is er voor zodat de text naar voren komt met een animatie
tl.to(".cover", {y: "-190%", duration: 0.8}, "-=1");
//dit is ervoor dat de img met een animatie naar boven komt.



