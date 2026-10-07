---
name: memoria-proyecto
description: Mantiene memoria persistente y compacta del estado, decisiones y progreso del proyecto Mixi-Mixi.
---

# Skill: Memoria del Proyecto

## Objetivo

Mantener información suficiente para que una sesión futura pueda
continuar el desarrollo de Mixi-Mixi sin depender del historial completo
de conversación de OpenClaw.

La memoria persistente del proyecto se almacena principalmente en:

- docs/ESTADO_ACTUAL.md
- docs/DECISIONES.md
- docs/ROADMAP.md
- docs/ARQUITECTURA.md

Estos archivos deben mantenerse actualizados, pero concisos.

## Antes de trabajar

Antes de comenzar una tarea de desarrollo:

1. leer `docs/ESTADO_ACTUAL.md`;
2. consultar `docs/ROADMAP.md` si la tarea pertenece a una fase;
3. consultar `docs/ARQUITECTURA.md` si afecta sistemas técnicos;
4. consultar `docs/DECISIONES.md` únicamente cuando sean relevantes
   decisiones anteriores;
5. inspeccionar el código relacionado con la tarea.

No asumir que una característica está implementada únicamente porque
aparece en el documento de diseño.

El código y `ESTADO_ACTUAL.md` determinan el estado real de
implementación.

## ESTADO_ACTUAL.md

`docs/ESTADO_ACTUAL.md` debe funcionar como una fotografía compacta
del proyecto.

Debe contener:

# Estado actual de Mixi-Mixi

## Fase actual

Indicar la fase del ROADMAP que se está desarrollando.

## Ultima tarea completada

Descripción breve.

## Funcionando

Lista corta de sistemas confirmados como funcionales.

## En desarrollo

Sistemas parcialmente implementados.

## Pendiente inmediato

Próximas tareas relevantes.

## Bugs conocidos

Únicamente errores actualmente conocidos y reproducibles.

## Pruebas realizadas

Pruebas importantes de la tarea reciente.

## Siguiente paso recomendado

Una tarea concreta que permita continuar el desarrollo.

## Ultima actualizacion

Fecha y resumen breve.

## Regla de compactacion

`ESTADO_ACTUAL.md` debe ser compacto.

No convertirlo en un diario completo.

No guardar:

- conversaciones;
- razonamientos extensos;
- logs completos;
- código fuente completo;
- errores ya solucionados sin valor histórico;
- explicaciones que puedan obtenerse leyendo el código.

Cuando información antigua deje de ser necesaria, resumirla o
eliminarla.

## DECISIONES.md

Registrar únicamente decisiones que puedan ser importantes en el
futuro.

Ejemplos:

- elección de arquitectura;
- elección del sistema de entrada;
- estructura de guardado;
- forma de manejar estadísticas;
- decisiones sobre prefabs;
- decisiones sobre animaciones;
- cambios importantes de alcance.

Formato recomendado:

## DEC-001 - Titulo

Fecha:

Contexto:

Decision:

Motivo:

Consecuencias:

No registrar decisiones triviales.

## ROADMAP.md

El ROADMAP representa el plan general.

No utilizarlo como registro diario.

Los estados permitidos para una fase son:

- PENDIENTE
- EN_PROGRESO
- BLOQUEADA
- COMPLETADA

Actualizar el estado únicamente cuando exista evidencia suficiente.

No marcar una fase como COMPLETADA si existen funciones principales
sin implementar.

## ARQUITECTURA.md

Debe explicar la arquitectura actual, no una arquitectura imaginaria.

Actualizarlo cuando cambien:

- sistemas principales;
- responsabilidades;
- dependencias importantes;
- flujo de datos;
- organización técnica relevante.

Evitar documentar cada método o variable.

El código contiene los detalles de implementación.

## Durante una tarea

No actualizar documentos de memoria después de cada pequeño cambio.

Trabajar primero sobre la unidad lógica correspondiente.

Actualizar la memoria cuando:

- se complete una funcionalidad;
- cambie una decisión importante;
- aparezca un bloqueo relevante;
- se descubra un bug que quedará pendiente;
- finalice una fase de trabajo.

## Después de una tarea

Antes de considerar terminada una tarea:

1. revisar qué cambió realmente;
2. verificar qué funciona;
3. identificar lo pendiente;
4. actualizar `ESTADO_ACTUAL.md`;
5. actualizar `DECISIONES.md` si hubo una decisión importante;
6. actualizar `ARQUITECTURA.md` si cambió la arquitectura;
7. actualizar `ROADMAP.md` si cambió el estado de una fase.

## Continuidad entre sesiones

Cuando una nueva sesión necesite continuar el proyecto, reconstruir
el contexto en este orden:

1. `docs/ESTADO_ACTUAL.md`
2. tarea solicitada por el usuario
3. archivos de código directamente relacionados
4. `docs/ARQUITECTURA.md` si es necesario
5. `docs/DECISIONES.md` si es necesario
6. `docs/DISENO_JUEGO.md` para reglas de diseño específicas
7. referencias visuales únicamente cuando la tarea sea visual

No cargar todos los documentos e imágenes automáticamente.

## Referencias visuales

No analizar las imágenes de `docs/referencias/` durante tareas que no
sean visuales.

Ejemplo:

Para corregir `SistemaVida.cs` no es necesario analizar:

- concept art de Chipotle;
- escenarios;
- enemigos;
- jefes;
- objetos.

Consultar imágenes únicamente cuando aporten información necesaria para
la tarea.

## Errores y logs

Guardar solamente información útil para continuar posteriormente.

En lugar de almacenar un log completo:

Incorrecto:

"500 líneas de Console..."

Correcto:

"Bug pendiente: NullReferenceException en ControladorJugador.cs al
reiniciar Nivel 1. Ocurre cuando ReferenciaHUD es null."

Una vez solucionado un bug, retirarlo de `Bugs conocidos`.

Si la solución produjo una decisión arquitectónica importante,
registrarla en `DECISIONES.md`.

## Evitar duplicacion

No duplicar información entre documentos.

Usar:

`DISENO_JUEGO.md`
para qué debe ser el juego.

`DIRECCION_ARTE.md`
para cómo debe verse.

`ARQUITECTURA.md`
para cómo está organizado técnicamente.

`ROADMAP.md`
para qué fases existen.

`ESTADO_ACTUAL.md`
para dónde se encuentra el desarrollo ahora.

`DECISIONES.md`
para explicar decisiones importantes y sus motivos.

## Regla final

La memoria debe permitir continuar el proyecto con el menor contexto
posible.

Priorizar información accionable sobre información histórica.

Si una información puede obtenerse rápidamente inspeccionando un archivo
pequeño del proyecto, normalmente no es necesario duplicarla en la
memoria.
