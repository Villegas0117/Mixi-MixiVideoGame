---
name: desarrollo-unity
description: Reglas de ingeniería para desarrollar y modificar Mixi-Mixi dentro del proyecto Unity existente.
---

# Skill: Desarrollo Unity

## Objetivo

Desarrollar Mixi-Mixi directamente sobre el proyecto Unity existente.

No crear otro proyecto Unity.

La raíz del proyecto es el directorio que contiene:

- Assets/
- Packages/
- ProjectSettings/

Antes de realizar cambios, inspeccionar el estado actual y reutilizar
lo que ya exista siempre que sea razonable.

## Version de Unity

El proyecto utiliza:

Unity 6000.3.23f1 LTS

Lenguaje principal:

C#

## Directorios administrados por Unity

No editar manualmente ni utilizar como fuente de contexto habitual:

- Library/
- Temp/
- Logs/
- UserSettings/

No recorrer estos directorios de forma recursiva salvo que exista
una razón concreta de diagnóstico.

No incluir su contenido completo en el contexto del modelo.

Los directorios principales que deben inspeccionarse son:

- Assets/
- Packages/
- ProjectSettings/
- docs/
- skills/

## Eficiencia de contexto

No inspeccionar todo el repositorio para cada tarea.

Antes de leer archivos:

1. identificar qué sistema está involucrado;
2. localizar los archivos relacionados;
3. leer únicamente esos archivos;
4. ampliar la búsqueda solo si es necesario.

Evitar:

- imprimir archivos completos innecesariamente;
- repetir contenido ya conocido;
- mostrar logs completos;
- recorrer Library/;
- recorrer Temp/;
- analizar archivos binarios sin necesidad;
- volver a analizar imágenes que no intervienen en la tarea.

Para errores de Unity, buscar primero el mensaje concreto y su stack trace
relevante.

## Idioma

Mantener español siempre que sea razonable.

Ejemplos:

- ControladorJugador
- MovimientoJugador
- SistemaVida
- SistemaComida
- ControladorEnemigo
- ControladorJefe
- GestorNivel
- GestorGuardado

Evitar acentos, ñ, espacios y caracteres especiales en identificadores
y nombres de archivos.

Los nombres pertenecientes a APIs de Unity y .NET deben conservarse.

Ejemplos:

- MonoBehaviour
- Rigidbody2D
- Collider2D
- Animator
- ScriptableObject
- Update
- FixedUpdate
- Awake
- Start

No traducir APIs oficiales.

## Arquitectura

Preferir componentes pequeños y con responsabilidades claras.

Evitar clases que controlen simultáneamente:

- movimiento;
- combate;
- vida;
- inventario;
- interfaz;
- guardado.

Separar responsabilidades cuando sea razonable.

Ejemplo conceptual:

Jugador
├── MovimientoJugador
├── CombateJugador
├── VidaJugador
├── SistemaComida
└── InventarioJugador

No aplicar esta estructura ciegamente si el proyecto existente ya tiene
una solución funcional equivalente.

## Reutilizacion

Antes de crear un sistema nuevo:

1. buscar si ya existe;
2. comprobar si puede extenderse;
3. evitar duplicación;
4. crear uno nuevo únicamente cuando sea necesario.

Los tres niveles deben compartir sistemas reutilizables.

No implementar tres versiones diferentes del mismo sistema solo porque
aparezca en tres escenas.

## Prefabs

Utilizar prefabs para objetos reutilizables como:

- jugador;
- enemigos;
- jefes;
- objetos;
- proyectiles;
- elementos repetitivos de interfaz.

Evitar duplicar manualmente configuraciones complejas entre escenas.

## Datos

Cuando resulte apropiado, separar configuración de comportamiento.

Considerar ScriptableObject para datos reutilizables como:

- estadísticas de enemigos;
- estadísticas de jefes;
- objetos;
- power-ups;
- parámetros configurables.

No utilizar ScriptableObject simplemente por utilizarlo.

## Fisica 2D

Mixi-Mixi es un videojuego 2D.

Preferir componentes 2D:

- Rigidbody2D
- Collider2D
- Physics2D

No introducir sistemas 3D salvo necesidad explícita.

Las operaciones relacionadas con física deben realizarse de acuerdo con
el ciclo de física de Unity.

## Movimiento

El movimiento del jugador debe permitir como mínimo:

- caminar;
- correr;
- saltar.

Debe diseñarse de forma que posteriormente puedan integrarse:

- agarre;
- interacción;
- combate;
- animaciones.

Evitar valores mágicos dispersos por el código.

Exponer parámetros ajustables de gameplay de forma razonable en el
Inspector o mediante datos configurables.

## Combate

Mantener separados, cuando resulte práctico:

- detección del ataque;
- aplicación de daño;
- salud;
- animación;
- efectos visuales;
- audio.

El sistema debe poder reutilizarse entre jugador, enemigos y jefes
cuando tenga sentido.

## Enemigos

Crear una base reutilizable para comportamientos comunes.

Los enemigos específicos pueden extender o componer comportamientos
diferentes.

No crear una arquitectura excesivamente compleja para enemigos simples.

## Jefes

Los jefes pueden tener comportamientos y fases específicas.

Compartir sistemas comunes como:

- vida;
- daño;
- detección;
- estados generales;

cuando sea razonable.

Mantener los ataques particulares de cada jefe desacoplados del resto.

## Interfaz

La lógica del gameplay no debe depender innecesariamente de elementos
visuales concretos del HUD.

El HUD debe reflejar el estado del juego.

Ejemplos:

SistemaVida
→ informa cambios
→ HUD actualiza barra

en lugar de hacer que toda la lógica de vida esté dentro de la interfaz.

## Guardado

No guardar referencias directas complejas de objetos de escena.

Persistir únicamente los datos necesarios.

El diseño contempla guardado automático en:

- comienzo de nivel;
- checkpoints;
- derrota de jefe.

No implementar sistemas de guardado adicionales fuera del alcance sin
autorización.

## Referencias conceptuales

Las imágenes dentro de:

docs/referencias/

son material de referencia.

No modificarlas.

No moverlas automáticamente a Assets/.

No asumir que están listas como sprites.

Cuando sea necesario producir un asset basado en una referencia:

1. conservar el original;
2. crear un archivo nuevo;
3. colocarlo dentro de Assets/Arte/;
4. utilizar PNG cuando se necesite transparencia;
5. mantener nombres descriptivos.

## Sprites

Para personajes animados, mantener separados conceptualmente:

- referencia original;
- sprites preparados;
- clips de animación;
- Animator Controller.

Ejemplo:

Assets/Arte/Personajes/Chipotle/
├── Sprites/
└── ...

Assets/Animaciones/Chipotle/
├── Clips/
└── Controladores/

No sobrescribir la imagen conceptual original.

## Escenas

Las escenas principales deben guardarse dentro de:

Assets/Escenas/

Usar nombres descriptivos.

Ejemplos:

MenuPrincipal.unity
Nivel_01_Barrio.unity
Nivel_02_Iglesia.unity
Nivel_03_Almacen.unity

No crear escenas duplicadas como:

Nivel1Final.unity
Nivel1Final2.unity
Nivel1AhoraSiFinal.unity

Utilizar control de versiones para recuperar estados anteriores.

## Cambios de codigo

Antes de modificar código existente:

1. leer el archivo;
2. entender su responsabilidad;
3. identificar quién lo utiliza;
4. determinar el cambio mínimo necesario.

No reescribir un archivo completo si un cambio localizado resuelve el
problema.

No realizar refactors no relacionados durante la corrección de un bug.

## Errores

Cuando exista un error:

1. capturar el mensaje exacto;
2. identificar archivo y línea cuando estén disponibles;
3. determinar la causa raíz;
4. corregir el cambio mínimo;
5. comprobar nuevamente;
6. continuar solo cuando el error esté resuelto o documentado.

No realizar cambios aleatorios intentando solucionar errores.

## Pruebas

Después de implementar una funcionalidad, verificar al menos:

- que compile;
- que no aparezcan errores nuevos;
- que la funcionalidad principal siga operando;
- que el cambio no rompa sistemas directamente relacionados.

Cuando una prueba requiera interacción manual dentro del editor,
indicar exactamente qué debe comprobar el usuario.

## Cambios grandes

Antes de:

- reorganizar arquitectura;
- eliminar scripts;
- cambiar sistemas centrales;
- reemplazar una implementación funcional;
- modificar muchas escenas;

explicar el motivo y verificar que exista un punto recuperable mediante
Git.

## Git

Realizar cambios en unidades lógicas pequeñas.

No incluir en Git archivos temporales generados por Unity.

Antes de operaciones destructivas comprobar el estado del repositorio.

No utilizar comandos destructivos como:

git reset --hard
git clean -fd
git checkout -- .

sin autorización explícita.

No sobrescribir trabajo del usuario.

## Alcance

Consultar la skill `proyecto-mixi-mixi` y la documentación del proyecto
para determinar qué debe implementarse.

No inventar nuevas características para "mejorar" el videojuego sin
autorización.

## Finalizacion de una tarea

Al terminar una tarea:

1. comprobar los archivos modificados;
2. comprobar errores relevantes;
3. indicar brevemente qué se cambió;
4. indicar cómo probarlo;
5. actualizar `docs/ESTADO_ACTUAL.md` cuando corresponda;
6. registrar decisiones arquitectónicas importantes en
   `docs/DECISIONES.md`.

No escribir resúmenes excesivamente largos si la información ya está
registrada en los archivos del proyecto.
