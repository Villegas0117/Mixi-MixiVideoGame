# Mixi-Mixi - Direccion de Arte

## Objetivo

Este documento define las reglas visuales generales de Mixi-Mixi.

Las imagenes almacenadas en:

`docs/referencias/`

son referencias conceptuales.

NO son automaticamente assets finales del videojuego.

Los originales deben conservarse sin modificaciones.

---

# Estilo general

Mixi-Mixi utiliza una estetica:

- 2D
- pixel art
- caricaturesca
- colorida
- legible
- expresiva

El juego debe mantener coherencia visual entre:

- personajes
- enemigos
- jefes
- objetos
- escenarios
- interfaz
- animaciones

Evitar mezclar estilos visuales incompatibles.

---

# Referencias y assets finales

Existen dos categorias diferentes.

## Referencias conceptuales

Ubicacion:

`docs/referencias/`

Sirven para definir:

- apariencia
- silueta
- colores
- personalidad
- vestimenta
- ambientacion
- composicion general

Nunca sobrescribir estos archivos.

## Assets utilizados por Unity

Ubicacion:

`Assets/Arte/`

Estos son los recursos preparados especificamente para el videojuego.

Cuando un recurso sea derivado de una referencia conceptual:

1. conservar la referencia original;
2. crear un archivo nuevo;
3. eliminar el fondo cuando corresponda;
4. utilizar transparencia cuando corresponda;
5. exportar como PNG;
6. mantener dimensiones coherentes;
7. preparar el recurso para Unity.

---

# Personajes

## Chipotle

Referencia principal:

`docs/referencias/personajes/15_Chipotle_Concept_Art.jpg`

Chipotle debe conservar una identidad visual consistente en todas sus
animaciones.

Características visuales fundamentales deben obtenerse de la referencia
conceptual y mantenerse entre frames.

No rediseñar al personaje arbitrariamente durante la creación de nuevas
poses.

Las futuras animaciones pueden incluir:

- idle
- caminar
- correr
- saltar
- caer
- ataque de garra
- bola de pelo
- bola de pelo cargada
- recoger objeto
- lanzar objeto
- recibir dano
- derrota

No es obligatorio implementar todas simultaneamente.

Crear solamente las necesarias para la fase actual del desarrollo.

---

# Piyu

Referencia:

`docs/referencias/personajes/16_Piyu_Concept_Art.jpg`

Piyu es una perrita chihuahua de pelaje completamente negro.

Mantener:

- pelaje negro
- proporciones reconocibles
- estilo pixel art compatible con Chipotle
- identidad consistente entre escenas

Piyu no necesita inicialmente un conjunto completo de animaciones de
gameplay porque no es personaje jugable.

---

# Iguana

Referencia:

`docs/referencias/personajes/17_Iguana.jpg`

NPC del Nivel 1.

Debe mantener el mismo lenguaje visual de personajes y enemigos.

---

# Enemigos

## Secuestradores

Referencia:

`docs/referencias/enemigos/18_Secuestradores_Bandidos.jpg`

Deben compartir identidad visual pero pueden existir variaciones que
permitan distinguir enemigos.

## Monaguillos

Referencia:

`docs/referencias/enemigos/20_Monaguillos.jpg`

Enemigos del Nivel 2.

Mantener coherencia con la ambientacion de iglesia y cementerio.

---

# Jefes

## Vagabundo

Referencia:

`docs/referencias/jefes/JefeNivel1Vagabundo.jpg`

Jefe del Nivel 1.

Su diseño debe funcionar visualmente dentro del escenario de basurero y
zona abandonada.

## Sacerdote Fidel

Referencia:

`docs/referencias/jefes/JefeNivel2SacerdoteFidel.jpg`

Jefe del Nivel 2.

Debe ser visualmente identificable respecto de los monaguillos.

## Lider de los secuestradores

Referencia:

`docs/referencias/jefes/JefeNivel3Secuestradores.jpg`

Jefe final.

Debe conservar relación visual con los secuestradores comunes pero ser
claramente reconocible como jefe.

---

# Escenarios

Las referencias de escenario establecen principalmente:

- atmosfera
- composicion
- arquitectura
- elementos caracteristicos
- paleta aproximada
- ambientacion

No deben utilizarse necesariamente como una unica imagen estatica de
fondo.

Cuando sea conveniente, los escenarios deben reconstruirse mediante:

- tiles
- sprites
- capas
- objetos independientes
- elementos decorativos
- foreground
- background

Esto permitirá crear niveles jugables y reutilizar elementos.

---

# Nivel 1

Referencia:

`docs/referencias/escenarios/Nivel1ConceptoBackground.jpg`

Ambientacion:

- casa
- barrio / colonia
- parque
- basurero
- zona abandonada

Sensacion general:

inicio de aventura y peligro moderado.

Debe ser visualmente claro para facilitar el aprendizaje inicial.

---

# Nivel 2

Referencia:

`docs/referencias/escenarios/Nivel2ConceptoBackground.jpg`

Ambientacion:

- calles
- iglesia
- cementerio

Debe diferenciarse claramente del Nivel 1.

La atmosfera puede ser ligeramente mas seria y oscura sin perder el
estilo caricaturesco.

---

# Nivel 3

Referencia:

`docs/referencias/escenarios/Nivel3ConceptoBackground.jpg`

Ambientacion:

- callejones
- zona industrial
- almacen abandonado

Debe transmitir una mayor sensacion de peligro y cierre de aventura.

Puede utilizar una atmosfera mas oscura que los niveles anteriores,
manteniendo legibilidad del gameplay.

---

# Objetos

Referencias:

`docs/referencias/objetos/`

Cada objeto debe ser fácilmente reconocible durante el gameplay.

Priorizar:

1. silueta;
2. contraste;
3. legibilidad;
4. consistencia visual.

Los objetos interactivos importantes deben distinguirse del fondo.

---

# Pixel art

Cuando se produzcan sprites definitivos:

- evitar escalado que genere suavizado;
- conservar bordes definidos;
- mantener resoluciones consistentes;
- utilizar Point filtering cuando corresponda;
- evitar compresion que degrade el pixel art.

La resolucion exacta y Pixels Per Unit deben definirse durante las
pruebas de integracion antes de producir masivamente los sprites.

No asumir una resolucion definitiva sin comprobar primero su aspecto
dentro de Unity.

---

# Animaciones

Las imagenes conceptuales pueden utilizarse como referencia para crear
frames nuevos.

No asumir que cada pose conceptual constituye automaticamente un frame
final.

Proceso recomendado:

Referencia conceptual
→ definir sprite base
→ eliminar fondo
→ normalizar dimensiones
→ crear frames
→ exportar PNG con transparencia
→ importar en Unity
→ configurar sprites
→ crear Animation Clip
→ configurar Animator
→ probar dentro del juego.

Mantener:

- tamaño aparente consistente;
- punto de apoyo consistente;
- orientación coherente;
- proporciones del personaje;
- paleta coherente.

---

# Estructura recomendada

Ejemplo para Chipotle:

`Assets/Arte/Personajes/Chipotle/Sprites/`

para imágenes utilizadas por Unity.

Animaciones:

`Assets/Animaciones/Chipotle/Clips/`

Controladores:

`Assets/Animaciones/Chipotle/Controladores/`

Aplicar una estructura equivalente cuando sea necesario para otros
personajes.

---

# Regla de produccion

No producir todos los sprites y animaciones del juego antes de validar
el pipeline.

Primero realizar una prueba completa con Chipotle:

1. preparar sprite base;
2. importar en Unity;
3. comprobar escala;
4. comprobar transparencia;
5. comprobar Pixel Per Unit;
6. probar movimiento;
7. crear una animacion sencilla;
8. comprobar calidad visual.

Una vez validado el proceso, utilizar las mismas reglas para producir el
resto del arte.

---

# Regla fundamental

Las referencias conceptuales definen la direccion visual.

Los assets finales deben adaptarse a las necesidades tecnicas del juego
sin perder la identidad de las referencias.

No modificar los originales.

No generar grandes cantidades de arte antes de validar el pipeline
visual dentro de Unity.
