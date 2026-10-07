# Mixi-Mixi - Diseno del Juego

## Fuente de verdad

Este documento resume el diseno funcional de Mixi-Mixi.

Las referencias visuales originales se encuentran en:

`docs/referencias/`

No ampliar el alcance del juego sin autorizacion.

---

## Informacion general

Nombre: Mixi-Mixi

Motor: Unity 6000.3.23f1 LTS

Genero:
Aventura de accion 2D con exploracion, plataformas ligeras y combate.

Plataformas:
- Windows
- Linux

Numero de niveles:
3

Duracion objetivo:
Hasta aproximadamente 15 minutos por nivel.

No contempla:
- mundo abierto
- multijugador

---

# Premisa

Chipotle es un gato domestico que vive con Piyu, una pequena perrita
chihuahua negra.

Mientras sus duenos estan fuera, tres secuestradores irrumpen en la
casa y se llevan a Piyu.

Chipotle presencia el secuestro y alcanza a identificar las placas del
vehiculo.

A partir de ese momento comienza su busqueda.

Cada nivel proporciona una nueva pista hasta localizar el almacen
abandonado donde Piyu permanece cautiva.

---

# Bucle principal

Explorar escenario
→ localizar pistas y recursos
→ combatir o evitar amenazas
→ gestionar Vida y Comida
→ utilizar objetos
→ derrotar jefe
→ obtener pista
→ desbloquear siguiente nivel.

---

# Jugador

## Chipotle

Personaje principal controlable.

Referencia:

`docs/referencias/personajes/15_Chipotle_Concept_Art.jpg`

Habilidades previstas:

- caminar
- correr
- saltar
- agarrar
- interactuar
- ataque de garra
- lanzar bola de pelo
- cargar bola de pelo
- lanzar ropa y objetos
- utilizar objetos
- utilizar power-ups

Armas:

- garras
- bolas de pelo
- objetos arrojables

---

# Piyu

Perrita chihuahua completamente negra.

Referencia:

`docs/referencias/personajes/16_Piyu_Concept_Art.jpg`

Piyu cumple una funcion narrativa.

No es personaje jugable en el alcance actual.

Aparece al comienzo de la historia y durante el rescate final.

---

# Controles

Movimiento:
A / D o flechas izquierda/derecha

Salto:
Espacio

Correr:
Shift

Agarrar / interactuar:
E

Ataque de garra:
J

Bola de pelo:
K

Mantener K:
cargar bola de pelo

Lanzar objeto equipado:
L

Mochila:
B

Pausa:
Esc

---

# Sistemas principales

## Vida

Recibir dano disminuye la Vida.

Al llegar a cero debe producirse la derrota del jugador.

## Comida

La Comida disminuye durante el juego o mediante acciones exigentes.

Una cantidad baja debe reducir la eficacia del personaje.

La pierna de pollo restaura Comida.

## Monedas

Recurso secundario recolectable.

No existe un sistema tradicional de puntuacion.

## Mochila

Permite almacenar o gestionar:

- consumibles
- objetos arrojables
- power-ups

## Power-ups

### Velocidad

Incrementa temporalmente la velocidad de Chipotle.

### Fuerza

Incrementa temporalmente el dano realizado por Chipotle.

---

# Objetos

## Pierna de pollo

Restaura Comida.

Referencia:

`docs/referencias/objetos/25_Pierna_de_Pollo.jpg`

## Moneda

Recurso secundario recolectable.

Referencia:

`docs/referencias/objetos/26_Moneda.jpg`

## Bola de pelo

Recurso relacionado con el ataque a distancia.

Referencia:

`docs/referencias/objetos/27_Carga_Bola_de_Pelo.jpg`

## Ropa arrojable

Puede recogerse y utilizarse como proyectil.

Referencia:

`docs/referencias/objetos/28_Ropa_Arrojable.jpg`

## Power-up de Velocidad

Referencia:

`docs/referencias/objetos/29_Power_Up_de_Velocidad.jpg`

## Power-up de Fuerza

Referencia:

`docs/referencias/objetos/30_Power_Up_de_Fuerza.jpg`

## Pistas

Objetos narrativos necesarios para progresar.

Referencia:

`docs/referencias/objetos/31_Pistas.jpg`

---

# Nivel 1 - La placa del coche

Referencia visual:

`docs/referencias/escenarios/Nivel1ConceptoBackground.jpg`

Zonas:

- casa
- colonia
- parque
- basurero / zona abandonada

Funcion:

Primer nivel y tutorial.

Debe introducir progresivamente:

- movimiento
- salto
- carrera
- agarre
- ataque
- Vida
- Comida
- recursos

Objetivos:

1. salir de la casa;
2. recorrer la colonia;
3. localizar pistas relacionadas con la placa;
4. superar enemigos;
5. llegar al basurero;
6. derrotar al Vagabundo;
7. liberar a la iguana.

La iguana revela que los secuestradores se dirigieron hacia la iglesia.

Jefe:

Vagabundo.

Referencia:

`docs/referencias/jefes/JefeNivel1Vagabundo.jpg`

Ataques previstos:

- lanzar mapaches
- lanzar bolsas de basura
- utilizar tambos como proyectiles u obstaculos

NPC:

Iguana.

Referencia:

`docs/referencias/personajes/17_Iguana.jpg`

Resultado:

Desbloqueo del Nivel 2.

---

# Nivel 2 - La pista de la iglesia

Referencia visual:

`docs/referencias/escenarios/Nivel2ConceptoBackground.jpg`

Zonas:

- calles
- veterinaria como referencia
- iglesia
- cementerio

Debe aumentar:

- cantidad de enemigos
- dificultad
- necesidad de combinar habilidades
- importancia de ataques a distancia
- uso de power-ups

Enemigos:

Monaguillos.

Referencia:

`docs/referencias/enemigos/20_Monaguillos.jpg`

Jefe:

Sacerdote Fidel.

Referencia:

`docs/referencias/jefes/JefeNivel2SacerdoteFidel.jpg`

Fase inicial:

Ataques mediante humo de incienso.

Segunda fase:

Agua santa que reduce temporalmente la velocidad de Chipotle.

Resultado:

Se descubre la ubicacion aproximada del almacen abandonado.

Desbloqueo del Nivel 3.

---

# Nivel 3 - El almacen abandonado

Referencia visual:

`docs/referencias/escenarios/Nivel3ConceptoBackground.jpg`

Zonas:

- zona cercana a la escuela
- callejones
- almacen abandonado

Debe combinar las mecanicas aprendidas:

- plataformas
- agarre
- combate cuerpo a cuerpo
- bola de pelo
- bola de pelo cargada
- objetos arrojables
- mochila
- power-ups

Enemigos:

Secuestradores.

Referencia:

`docs/referencias/enemigos/18_Secuestradores_Bandidos.jpg`

Jefe final:

Lider de los secuestradores.

Referencia:

`docs/referencias/jefes/JefeNivel3Secuestradores.jpg`

Comportamiento previsto:

- ataques directos
- lanzamiento de objetos
- llamada de refuerzos

Resultado:

Chipotle libera a Piyu.

Se reproduce la cinemática final y posteriormente los creditos.

---

# Progresion narrativa

Cinematica inicial
→ secuestro de Piyu
→ placa del vehiculo
→ Nivel 1
→ Vagabundo
→ Iguana
→ pista de iglesia
→ Nivel 2
→ Sacerdote Fidel
→ pista del almacen
→ Nivel 3
→ Lider de los secuestradores
→ rescate de Piyu
→ cinemática final
→ creditos.

---

# Guardado

Guardado local automatico previsto en:

- comienzo de nivel
- checkpoint
- derrota de jefe

El menu principal debe permitir:

`Continuar`

para cargar el ultimo progreso.

No se requiere guardado manual para el alcance actual.

---

# Estados del juego

- Menu principal
- Cinematica inicial
- Juego activo
- Pausa
- Mochila
- Combate con jefe
- Derrota
- Transicion entre niveles
- Cinematica final
- Creditos

---

# Interfaz

## Menu principal

- Jugar
- Continuar
- Controles
- Salir

## HUD

Debe mostrar:

- Vida
- Comida
- monedas
- objeto o power-up equipado
- objetivo actual

## Pausa

- Continuar
- Reiniciar desde checkpoint
- Controles
- Volver al menu

## Mochila

Panel superpuesto sobre la escena.

## Derrota

- Reintentar
- Volver al menu

---

# Audio previsto

Musica:

- M01 Barrio / Parque
- M02 Jefe Vagabundo
- M03 Iglesia / Cementerio
- M04 Jefe Fidel
- M05 Almacen
- M06 Jefe final
- M07 Rescate / Creditos

Efectos previstos:

- salto
- garra
- bola de pelo
- recoger objeto
- recibir dano
- basura / mapache
- pista encontrada
- incienso
- agua
- ralentizacion
- power-up
- puerta
- cadenas
- golpe del jefe
- refuerzos
- rescate

---

# Alcance

El objetivo es producir una experiencia corta pero completa.

Prioridad:

1. tres niveles jugables;
2. sistemas principales funcionales;
3. tres jefes;
4. progresion completa;
5. rescate de Piyu;
6. estabilidad;
7. presentacion y pulido.

No implementar caracteristicas adicionales importantes sin autorizacion.
