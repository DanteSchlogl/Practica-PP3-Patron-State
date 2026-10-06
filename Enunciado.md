# Práctica de examen — Patrón State

**INSTITUTO UNIVERSITARIO LEONARDO DA VINCI**
Carrera: ANALISTA EN SISTEMAS INFORMÁTICOS
Asignatura: PRÁCTICAS PROFESIONALIZANTES III

Mesa: Práctica de examen
Turno: Noche

**Temas a evaluar:** El producto y el proceso del desarrollo de software. Gestión del Riesgo.
Patrones respecto al nivel de abstracción. Concepto de patrón de diseño. Relación entre los
patrones de diseño y el desarrollo de software. Análisis y detección de los problemas que
pueden ser resueltos por patrones.

**Requisitos para aprobar:** 70% del ejercicio realizado (el programa debe funcionar) y debe
aplicar patrones donde se considere necesario.

**Recomendaciones:** a) Lea todo el parcial antes de comenzar a responder. b) Desarrolle una
redacción clara y precisa contestando lo que la pregunta requiere. c) Observe la ortografía.

---

## Ejercicio Práctico

Disponemos de una clase `Pedido` que modela un pedido de una tienda online, cuya forma es la
siguiente:

```
Clase PEDIDO
  Numero: int        – Número de pedido
  Total: decimal     – Importe total del pedido
  Estado             – El estado actual del pedido
  Historial          – Registro de los cambios de estado
```

Un pedido puede estar en **cinco estados**:

| Estado | Significado |
|---|---|
| `Pendiente` | Recién creado, todavía no se confirmó |
| `Confirmado` | El pago se acreditó |
| `Enviado` | Salió del depósito |
| `Entregado` | Llegó al cliente |
| `Cancelado` | Se anuló |

Y las **acciones** que se pueden hacer dependen del estado actual:

| Acción | Cuándo se permite |
|---|---|
| `Confirmar()` | Sólo si está **Pendiente** |
| `Enviar()` | Sólo si está **Confirmado** |
| `Entregar()` | Sólo si está **Enviado** |
| `Cancelar()` | Sólo si está **Pendiente** o **Confirmado** (un pedido ya enviado no se puede cancelar) |

Si se intenta una acción que no corresponde al estado actual, el programa debe **avisar del
error sin cerrarse**.

Además:

1. Cada cambio de estado debe guardarse en un **archivo de historial del pedido** (un archivo
   por pedido, por ejemplo `historial_pedido_1.txt`), con la **fecha y hora** de cada cambio.
2. La consola debe mostrar el **estado actual** y **qué acciones se pueden hacer** en ese momento.
3. Toda la solución debe implementarse con **arquitectura de 4 capas** (UI, BLL, DAL, Domain).

---

## Criterios de Evaluación (Total 10 puntos)

### 1. Implementación del patrón State (2 puntos)

- Cada estado es una **clase propia**, no un `switch` gigante dentro de `Pedido`.
- El pedido **delega** la acción en el estado actual.
- Las transiciones inválidas se rechazan con una excepción.

### 2. Reglas de la máquina de estados (2 puntos)

- Todas las transiciones válidas funcionan.
- No se puede cancelar un pedido enviado ni entregado.
- Validación del total (debe ser mayor a 0).

### 3. Persistencia del historial (2 puntos)

- Un archivo por pedido.
- Fecha y hora en cada cambio.
- Se agrega al final del archivo, no se pisa lo anterior.
- Manejo de errores de archivo.

### 4. Salida por consola (2 puntos)

- Se ve el estado actual y las acciones permitidas.
- Los errores se muestran claros y el programa sigue funcionando.

### 5. Calidad de Código (2 puntos)

- Manejo de excepciones.
- Arquitectura en capas y diseño general.
- Comentarios y documentación.
