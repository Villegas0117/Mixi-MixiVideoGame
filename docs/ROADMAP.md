# Roadmap de desarrollo - Mixi-Mixi

## Objetivo

Desarrollar Mixi-Mixi incrementalmente.

No avanzar automaticamente a una fase posterior si la fase actual
presenta errores importantes o no ha sido validada.

Estados posibles:

- PENDIENTE
- EN_PROGRESO
- BLOQUEADA
- COMPLETADA

---

# Fase 0 - Preparacion y arquitectura

Estado: EN_PROGRESO

Objetivo:

Preparar una base tecnica estable antes de implementar gameplay.

Tareas:

- [x] Crear estructura inicial de carpetas.
- [x] Organizar referencias conceptuales.
- [x] Crear documentacion de diseno.
- [x] Crear documentacion de direccion de arte.
- [x] Crear Skills iniciales.
- [x] Crear memoria persistente del proyecto.
- [ ] Configurar `.gitignore`.
- [ ] Inicializar o verificar repositorio Git.
- [ ] Crear punto de restauracion inicial.
- [ ] Auditar proyecto Unity existente.
- [ ] Revisar Packages y ProjectSettings relevantes.
- [ ] Definir arquitectura tecnica inicial.
- [ ] Actualizar `ARQUITECTURA.md`.
- [ ] Confirmar que Unity abre el proyecto sin errores.

Criterio de salida:

- proyecto abre correctamente;
- Git esta configurado;
- arquitectura inicial documentada;
- no existen errores bloqueantes.

---

# Fase 1 - Prototipo jugable de Chipotle

Estado: PENDIENTE

Objetivo:

Conseguir un personaje jugable funcional antes de construir niveles
completos.

Tareas:

- [ ] Crear o preparar prefab de Chipotle.
- [ ] Implementar movimiento horizontal.
- [ ] Implementar salto.
- [ ] Implementar carrera.
- [ ] Configurar deteccion de suelo.
- [ ] Configurar colisiones.
- [ ] Configurar camara.
- [ ] Crear escena de pruebas.
- [ ] Integrar sprite provisional o definitivo.
- [ ] Crear animacion Idle.
- [ ] Crear animacion de caminar.
- [ ] Conectar Animator con movimiento.
- [ ] Probar controles y fisica.

Criterio de salida:

Chipotle puede desplazarse, correr y saltar de forma estable en una
escena de pruebas.

---

# Fase 2 - Sistemas base de gameplay

Estado: PENDIENTE

Objetivo:

Implementar los sistemas reutilizables por los tres niveles.

Tareas:

- [ ] Sistema de Vida.
- [ ] Recepcion de dano.
- [ ] Sistema de Comida.
- [ ] Monedas.
- [ ] Interaccion mediante E.
- [ ] Recoger objetos.
- [ ] Ataque de garra.
- [ ] Bola de pelo.
- [ ] Carga de bola de pelo.
- [ ] Objetos arrojables.
- [ ] Mochila basica.
- [ ] Power-up de velocidad.
- [ ] Power-up de fuerza.
- [ ] HUD basico.
- [ ] Pausa.
- [ ] Derrota.
- [ ] Sistema basico de objetivos.
- [ ] Sistema de checkpoints.

Criterio de salida:

Los sistemas pueden probarse independientemente del diseño final de los
niveles y no presentan errores bloqueantes.

---

# Fase 3 - Nivel 1

Estado: PENDIENTE

Objetivo:

Construir el primer nivel completo y validar el flujo principal del
juego.

Contenido:

- casa;
- colonia;
- parque;
- basurero;
- tutorial;
- pistas;
- enemigos;
- Vagabundo;
- Iguana.

Tareas:

- [ ] Construir escenario jugable.
- [ ] Implementar progresion del tutorial.
- [ ] Colocar recursos y objetos.
- [ ] Implementar enemigos necesarios.
- [ ] Implementar pistas.
- [ ] Implementar objetivo actual.
- [ ] Implementar encuentro con Vagabundo.
- [ ] Implementar ataques del Vagabundo.
- [ ] Implementar liberacion de la Iguana.
- [ ] Implementar dialogo/pista hacia la iglesia.
- [ ] Implementar checkpoint.
- [ ] Implementar final del nivel.
- [ ] Desbloquear Nivel 2.
- [ ] Realizar pruebas completas del nivel.

Criterio de salida:

Nivel 1 puede jugarse desde su inicio hasta obtener la pista de la
iglesia.

---

# Fase 4 - Guardado y flujo general

Estado: PENDIENTE

Objetivo:

Permitir continuidad entre sesiones y establecer el flujo completo de
la aplicacion.

Tareas:

- [ ] Menu principal.
- [ ] Jugar.
- [ ] Continuar.
- [ ] Controles.
- [ ] Salir.
- [ ] Guardado al comenzar nivel.
- [ ] Guardado en checkpoint.
- [ ] Guardado al derrotar jefe.
- [ ] Carga de progreso.
- [ ] Reinicio desde checkpoint.
- [ ] Transiciones entre escenas.

Criterio de salida:

El usuario puede cerrar el juego, abrirlo nuevamente y continuar desde
el progreso guardado esperado.

---

# Fase 5 - Nivel 2

Estado: PENDIENTE

Objetivo:

Construir el segundo nivel reutilizando los sistemas existentes.

Contenido:

- calles;
- iglesia;
- cementerio;
- monaguillos;
- Sacerdote Fidel.

Tareas:

- [ ] Construir escenario.
- [ ] Incrementar densidad de enemigos.
- [ ] Implementar Monaguillos.
- [ ] Integrar mochila y power-ups en el diseño del nivel.
- [ ] Diseñar encuentros que utilicen ataques a distancia.
- [ ] Implementar Sacerdote Fidel.
- [ ] Implementar ataque de incienso.
- [ ] Implementar segunda fase.
- [ ] Implementar agua santa.
- [ ] Implementar ralentizacion temporal.
- [ ] Implementar pista del almacen.
- [ ] Implementar checkpoints.
- [ ] Desbloquear Nivel 3.
- [ ] Realizar pruebas completas del nivel.

Criterio de salida:

Nivel 2 puede jugarse completamente y termina proporcionando la pista
hacia el almacen abandonado.

---

# Fase 6 - Nivel 3 y rescate

Estado: PENDIENTE

Objetivo:

Construir el nivel final y cerrar la historia.

Contenido:

- zona cercana a la escuela;
- callejones;
- almacen abandonado;
- secuestradores;
- lider de los secuestradores;
- Piyu.

Tareas:

- [ ] Construir escenario.
- [ ] Implementar Secuestradores.
- [ ] Aumentar dificultad respecto al Nivel 2.
- [ ] Combinar las mecanicas aprendidas.
- [ ] Implementar acceso al almacen.
- [ ] Implementar encuentro final.
- [ ] Implementar Lider de los secuestradores.
- [ ] Implementar ataques directos.
- [ ] Implementar lanzamiento de objetos.
- [ ] Implementar llamada de refuerzos.
- [ ] Implementar derrota del jefe.
- [ ] Implementar rescate de Piyu.
- [ ] Implementar final del nivel.
- [ ] Realizar pruebas completas.

Criterio de salida:

El jugador puede completar los tres niveles y rescatar a Piyu.

---

# Fase 7 - Narrativa y presentacion

Estado: PENDIENTE

Objetivo:

Completar los elementos narrativos y de presentación.

Tareas:

- [ ] Cinematica inicial.
- [ ] Secuestro de Piyu.
- [ ] Presentacion de la placa.
- [ ] Transiciones narrativas.
- [ ] Cinematica final.
- [ ] Rescate de Piyu.
- [ ] Creditos.
- [ ] Revisar textos.
- [ ] Revisar interfaz.

Criterio de salida:

La historia puede entenderse desde el inicio hasta los creditos.

---

# Fase 8 - Arte y audio final

Estado: PENDIENTE

Objetivo:

Sustituir recursos provisionales y completar la presentacion audiovisual.

Tareas:

- [ ] Validar pipeline definitivo de pixel art.
- [ ] Completar sprites necesarios de Chipotle.
- [ ] Completar sprites necesarios de Piyu.
- [ ] Completar enemigos.
- [ ] Completar jefes.
- [ ] Completar objetos.
- [ ] Completar escenarios.
- [ ] Completar animaciones necesarias.
- [ ] Integrar musica.
- [ ] Integrar efectos de sonido.
- [ ] Ajustar volumenes.
- [ ] Revisar coherencia visual.

Criterio de salida:

No quedan placeholders importantes visibles durante una partida normal.

---

# Fase 9 - Integracion y pruebas finales

Estado: PENDIENTE

Objetivo:

Preparar una version entregable estable.

Tareas:

- [ ] Probar partida completa.
- [ ] Corregir bugs bloqueantes.
- [ ] Corregir bugs importantes.
- [ ] Revisar balance.
- [ ] Revisar densidad de enemigos.
- [ ] Revisar dificultad progresiva.
- [ ] Revisar checkpoints.
- [ ] Revisar guardado.
- [ ] Revisar HUD.
- [ ] Revisar menus.
- [ ] Revisar audio.
- [ ] Revisar rendimiento.
- [ ] Probar build Windows.
- [ ] Probar build Linux.
- [ ] Preparar version final.
- [ ] Actualizar documentacion.

Criterio de salida:

Mixi-Mixi puede jugarse de principio a fin sin errores bloqueantes y
existen builds funcionales para las plataformas objetivo.

---

# Regla de avance

Trabajar prioritariamente sobre la fase marcada `EN_PROGRESO`.

No iniciar varias fases grandes simultaneamente salvo que exista una
dependencia tecnica justificada.

Al completar una fase:

1. verificar sus criterios de salida;
2. actualizar este documento;
3. actualizar `ESTADO_ACTUAL.md`;
4. registrar decisiones importantes;
5. crear un punto recuperable en Git;
6. continuar con la siguiente fase autorizada.

---

# Regla de alcance

Este roadmap organiza el desarrollo.

No autoriza agregar caracteristicas que no existan en
`DISENO_JUEGO.md`.

Si una nueva caracteristica parece conveniente, proponerla antes de
implementarla.
