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
