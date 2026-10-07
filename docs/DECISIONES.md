# Decisiones tecnicas de Mixi-Mixi

Este documento registra decisiones tecnicas o arquitectonicas que sean
importantes para continuar el proyecto en sesiones futuras.

No registrar decisiones triviales.

---

## DEC-001 - Idioma y nombres del proyecto

Fecha: 06/10/2026

Contexto:

Se desea mantener el proyecto comprensible y consistente en español.

Decision:

Utilizar español siempre que sea razonable para carpetas, scripts,
clases, variables, metodos, escenas, prefabs y assets propios.

Evitar espacios, acentos, ñ y caracteres especiales en nombres tecnicos.

Las APIs oficiales de Unity y .NET conservan sus nombres originales.

Motivo:

Mantener una estructura legible en español sin introducir problemas
innecesarios de compatibilidad con herramientas.

---

## DEC-002 - Separacion de referencias y assets

Fecha: 06/10/2026

Decision:

Las imagenes conceptuales originales permanecen en:

`docs/referencias/`

Los recursos preparados para utilizarse dentro del videojuego se
almacenan en:

`Assets/Arte/`

Los originales no deben modificarse.

Motivo:

Separar claramente concept art de assets de produccion.

---

## DEC-003 - Conservar la base URP 2D e Input System

Fecha: 07/10/2026. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Contexto: La auditoria confirma URP 17.3.0 con Renderer 2D mediante QualitySettings e Input System 1.20.0 exclusivo. El asset de acciones existente es generico y no hay gameplay propio que migrar. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Decision: Mantener esta base y adaptar progresivamente el asset de acciones existente al diseno, en las fases autorizadas. No cambiar a Built-in, activar entrada legacy ni introducir otro sistema paralelo sin una necesidad comprobada. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Motivo: Reutilizar herramientas ya declaradas y suficientes para el juego 2D, evitando migraciones y dependencias innecesarias. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Consecuencias: Validar primero en Unity el render, referencias y paquetes. Las acciones de plantilla no cuentan como controles jugables implementados. Esta decision documental no modifica assets ni certifica funcionamiento. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

---

## DEC-004 - Composicion reutilizable e implementacion incremental

Fecha: 07/10/2026. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Contexto: Existen carpetas preparadas, pero no scripts ni prefabs propios. El alcance tiene tres niveles que comparten jugador, combate, recursos e interfaz. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Decision: Usar componentes pequenos, prefabs compartidos, referencias explicitas y eventos locales. Separar reglas de gameplay, presentacion y persistencia; preferir parametros del Inspector y usar ScriptableObject solo cuando haya configuracion realmente reutilizada. Implementar por fase, sin crear un framework global ni esqueletos de todos los sistemas. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Motivo: Evitar duplicacion entre niveles y sobrearquitectura antes de validar el prototipo. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->

Consecuencias: ARQUITECTURA.md separa base real de propuesta futura. Estrategias de escenas/datos/guardado alli descritas aun no son sistemas existentes. Ninguna implementacion de Fase 1 queda autorizada por esta auditoria. <!-- project: path:/run/media/eber/PopOS/UnityProjectsUTP/Mixi-Mixi2D -->
