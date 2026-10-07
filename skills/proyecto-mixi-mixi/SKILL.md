---
name: proyecto-mixi-mixi
description: Reglas, alcance y fuente de verdad para desarrollar el videojuego 2D Mixi-Mixi.
---

# Skill: Proyecto Mixi-Mixi

## Propósito

Esta skill contiene las reglas fundamentales del proyecto Mixi-Mixi.

Antes de realizar cambios importantes en el proyecto, consultar la
documentación disponible en `docs/`.

La documentación del proyecto tiene prioridad sobre suposiciones del
agente.

## Proyecto

Nombre: Mixi-Mixi

Motor:
Unity 6000.3.23f1 LTS

Lenguaje:
C#

Plataformas objetivo:
- Windows
- Linux

Género:
Aventura de acción 2D con exploración, plataformas ligeras y combate.

## Idioma del proyecto

Mantener nombres en español siempre que sea razonable.

Usar español para:

- carpetas
- escenas
- prefabs
- scripts propios
- clases propias
- variables
- métodos
- assets
- documentación

Para nombres técnicos propios de Unity o de APIs externas conservar
el nombre oficial cuando traducirlo pueda generar confusión.

Evitar en nombres de archivos, carpetas, clases y recursos:

- espacios
- acentos
- ñ
- caracteres especiales

Ejemplos correctos:

`ControladorJugador.cs`
`SistemaVida.cs`
`Nivel_01_Barrio.unity`
`chipotle_caminar_01.png`

## Fuente de verdad

Consultar cuando corresponda:

- `docs/DISENO_JUEGO.md`
- `docs/DIRECCION_ARTE.md`
- `docs/ARQUITECTURA.md`
- `docs/ROADMAP.md`
- `docs/ESTADO_ACTUAL.md`
- `docs/DECISIONES.md`

Las imágenes conceptuales están en:

`docs/referencias/`

Estas imágenes son referencias visuales y NO deben considerarse
automáticamente assets finales del juego.

Los assets preparados para Unity deben colocarse dentro de:

`Assets/Arte/`

Nunca modificar, recortar, reemplazar o eliminar las imágenes originales
de `docs/referencias/`.

## Personajes principales

### Chipotle

Protagonista jugable.

Es un gato doméstico pequeño, ágil y expresivo.

Referencia:

`docs/referencias/personajes/15_Chipotle_Concept_Art.jpg`

### Piyu

Compañera de Chipotle y objetivo del rescate.

Es una perrita chihuahua de pelaje completamente negro.

Referencia:

`docs/referencias/personajes/16_Piyu_Concept_Art.jpg`

Piyu no es un personaje jugable en el alcance actual.

## Estructura narrativa

El juego tiene exactamente tres niveles principales.

### Nivel 1

Barrio, colonia, parque y zona de basurero.

Jefe:
Vagabundo.

Objetivo narrativo:
obtener la pista que conduce hacia la iglesia.

### Nivel 2

Calles, iglesia y cementerio.

Jefe:
Sacerdote Fidel.

Objetivo narrativo:
obtener la ubicación aproximada del almacén abandonado.

### Nivel 3

Zona cercana a la escuela, callejones y almacén abandonado.

Jefe:
Lider de los secuestradores.

Objetivo:
rescatar a Piyu.

## Bucle principal

Explorar
→ encontrar pistas y recursos
→ combatir o evitar amenazas
→ gestionar Vida y Comida
→ derrotar al jefe
→ desbloquear la siguiente zona.

## Controles definidos

A / D o flechas:
movimiento horizontal.

Espacio:
saltar.

Shift:
correr.

E:
agarrar o interactuar.

J:
ataque de garra.

K:
bola de pelo.
Mantener presionado permite cargarla.

L:
lanzar ropa u objeto equipado.

B:
abrir/cerrar mochila.

Esc:
pausa.

## Sistemas principales

El proyecto contempla:

- movimiento
- salto
- carrera
- interacción
- combate cuerpo a cuerpo
- ataque a distancia
- objetos arrojables
- Vida
- Comida
- monedas
- inventario/mochila
- power-ups
- enemigos
- jefes
- objetivos
- pistas
- checkpoints
- guardado automático
- HUD
- pausa
- derrota
- transición entre niveles
- cinemática inicial
- cinemática final
- créditos

## Control de alcance

No añadir automáticamente:

- nuevos niveles
- multijugador
- mundo abierto
- nuevas mecánicas principales
- sistemas online
- contenido fuera del diseño establecido

Si surge una mejora interesante que no forma parte del alcance,
documentarla antes de implementarla.

## Principio de desarrollo

Priorizar en este orden:

1. estabilidad
2. jugabilidad
3. completar el alcance
4. mantenibilidad
5. presentación visual
6. características opcionales

No sacrificar sistemas funcionales para implementar mejoras cosméticas.

## Regla fundamental

Antes de implementar algo:

1. revisar qué existe;
2. consultar la documentación relevante;
3. inspeccionar únicamente los archivos necesarios;
4. identificar dependencias;
5. realizar el cambio mínimo razonable.

Después de implementar:

1. comprobar errores;
2. probar la funcionalidad afectada;
3. registrar cambios relevantes;
4. actualizar el estado del proyecto.
