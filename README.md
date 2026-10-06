# Práctica PP3 — Patrón State aplicado a la clase `Pedido`

| Dato | Valor |
|---|---|
| Instituto | Instituto Universitario Leonardo Da Vinci |
| Carrera | Analista en Sistemas Informáticos |
| Asignatura | Prácticas Profesionalizantes III |
| Instancia | Práctica de examen |
| Alumno | Dante Schlögl |
| Lenguaje / IDE | C# / Visual Studio |
| Framework | .NET 8 |
| Arquitectura | 4 capas: UI – BLL – DAL – Domain |
| Patrón principal | **State** |

El enunciado completo está en [`Enunciado.md`](Enunciado.md).

---

## 1. Consigna (resumen)

Un `Pedido` de tienda online que puede estar en cinco estados —`Pendiente`, `Confirmado`,
`Enviado`, `Entregado` y `Cancelado`— y que acepta cuatro acciones —`Confirmar()`, `Enviar()`,
`Entregar()` y `Cancelar()`—, pero **cada acción depende del estado actual**:

| Acción | Permitida desde |
|---|---|
| `Confirmar()` | Pendiente |
| `Enviar()` | Confirmado |
| `Entregar()` | Enviado |
| `Cancelar()` | Pendiente, Confirmado |

Si la acción no corresponde, se rechaza con una excepción y el programa sigue andando. Cada
cambio se guarda en un archivo de historial del pedido, y todo va en 4 capas.

---

## 2. Cómo ejecutar

### Desde Visual Studio

1. Abrir `PedidoApp.sln`.
2. Verificar que `PedidoApp.UI` sea el proyecto de inicio.
3. `F5` (o `Ctrl+F5`).

Corre una **demostración automática** y después un **menú** para probar a mano.

### Desde la línea de comandos

```bash
dotnet build PedidoApp.sln -c Release

# Demostración + menú
dotnet run --project PedidoApp.UI -c Release

# Sólo la demostración, sin menú
dotnet run --project PedidoApp.UI -c Release -- --demo
```

**Requisito:** .NET SDK 8.0 o superior. El historial se genera en la carpeta `Historiales/`.

---

## 3. Arquitectura en 4 capas

```
        +-----------------------------------------------+
        |                PedidoApp.UI                   |
        |   Program                                     |
        +----------------------+------------------------+
                               |  usa
                               v
        +-----------------------------------------------+
        |               PedidoApp.BLL                   |
        |   ServicioPedido, FabricaPedido                |
        +----------------------+------------------------+
                               |  usa
                               v
        +-----------------------------------------------+
        |               PedidoApp.DAL                   |
        |   IRepositorioHistorial,                       |
        |   RepositorioHistorialArchivo, formateadores    |
        +----------------------+------------------------+
                               |  usa
                               v
        +-----------------------------------------------+
        |              PedidoApp.Domain                 |
        |   Pedido, IEstadoPedido, los 5 estados,        |
        |   ResumenPedido y las excepciones              |
        +-----------------------------------------------+
```

| Capa | Proyecto | Responsabilidad | Depende de |
|---|---|---|---|
| **Domain** | `PedidoApp.Domain` | El modelo y el **patrón State**: `Pedido` como contexto y una clase por cada estado. | *(ninguna)* |
| **DAL** | `PedidoApp.DAL` | Persistencia del historial: escritura en archivo y formatos. Expone `IRepositorioHistorial`. | Domain |
| **BLL** | `PedidoApp.BLL` | `ServicioPedido` (única puerta de entrada para la UI) y la fábrica que arma todo. | Domain, DAL |
| **UI** | `PedidoApp.UI` | La consola: demostración automática y menú. | BLL, Domain |

Las dependencias van **en un solo sentido** (UI → BLL → DAL → Domain). La UI nunca crea
repositorios ni conoce el formato del archivo: le pide el servicio a `FabricaPedido`.

---

## 4. Patrones de diseño aplicados

### 4.1 State (el patrón principal)

**El problema:** el pedido se comporta distinto según el estado. Si toda la lógica estuviera
en `Pedido`, cada acción tendría un `switch` enorme que pregunta el estado actual, y agregar un
estado nuevo obligaría a tocar todos esos `switch`.

**La solución:** el patrón **State**.

| Rol del patrón | Clase |
|---|---|
| **Contexto** | `Pedido` |
| **Estado** (interfaz) | `IEstadoPedido` |
| **Estados concretos** | `EstadoPendiente`, `EstadoConfirmado`, `EstadoEnviado`, `EstadoEntregado`, `EstadoCancelado` |
| **Base con las acciones que no van** | `EstadoPedidoBase` |

Cómo funciona:

- `Pedido` guarda un `IEstadoPedido` y **delega**: `Confirmar()` llama a `_estado.Confirmar(this)`. No hay ningún `switch`.
- Cada estado concreto **sólo escribe las transiciones que permite**. Por ejemplo `EstadoPendiente` implementa `Confirmar` y `Cancelar`, y nada más.
- `EstadoPedidoBase` implementa las cuatro acciones lanzando `TransicionInvalidaException`, así los estados que no permiten una acción no tienen que escribir nada.
- Cuando la acción es válida, el estado le dice al pedido cuál es el estado nuevo con `CambiarEstado(...)`.
- `AccionesPermitidas` sale del propio estado, así la consola muestra qué se puede hacer sin adivinar.

### 4.2 Strategy (el formato del historial)

Con la interfaz `IFormateadorHistorial` y dos implementaciones intercambiables
(`FormateadorTextoPlano` y `FormateadorDetallado`). El repositorio sabe **dónde** guardar pero
no **con qué formato**: eso lo decide el formateador que se le pasa.

### 4.3 Factory Method (el armado)

`FabricaPedido.Crear(...)` arma el repositorio y el formato y devuelve un `ServicioPedido`
listo. Así la UI no necesita conocer la capa DAL.

### 4.4 ¿Por qué no otros patrones?

No se agregaron patrones que el problema no pedía. En particular **no** hizo falta Observer:
acá no hay varios interesados en enterarse de un cambio, hay **un objeto que cambia de
comportamiento**, y eso es exactamente State.

---

## 5. La máquina de estados

```
  Pendiente ---Confirmar()---> Confirmado ---Enviar()---> Enviado ---Entregar()---> Entregado
      |                            |
      | Cancelar()                 | Cancelar()
      v                            v
  Cancelado                    Cancelado
```

- Desde **Enviado** ya **no** se puede cancelar.
- **Entregado** y **Cancelado** son estados finales: no se puede hacer nada más.
- Todo pedido nuevo arranca en **Pendiente**.

| Estado actual | `Confirmar()` | `Enviar()` | `Entregar()` | `Cancelar()` | Acciones permitidas |
|---|---|---|---|---|---|
| Pendiente | → Confirmado | ✗ | ✗ | → Cancelado | confirmar, cancelar |
| Confirmado | ✗ | → Enviado | ✗ | → Cancelado | enviar, cancelar |
| Enviado | ✗ | ✗ | → Entregado | ✗ | entregar |
| Entregado | ✗ | ✗ | ✗ | ✗ | ninguna |
| Cancelado | ✗ | ✗ | ✗ | ✗ | ninguna |

### Validación

El total se valida en el constructor del pedido: si no es mayor a 0 lanza
`TotalInvalidoException`. La validación está en el **dominio**, así se aplica sin importar
desde qué capa se cree el pedido.

---

## 6. El historial

Un archivo de texto **por pedido**:

```
Historiales/
├── historial_pedido_1.txt
└── historial_pedido_2.txt
```

Formato por omisión (texto plano):

```
[06/10/2026 08:27:31] Pedido 1 - $1500 - Pendiente
[06/10/2026 08:27:31] Pedido 1 - $1500 - Confirmado
[06/10/2026 08:27:31] Pedido 1 - $1500 - Enviado
[06/10/2026 08:27:31] Pedido 1 - $1500 - Entregado
```

Formato alternativo (detallado):

```
--- cambio de estado ---
Fecha: 06/10/2026 08:27:31
Pedido: 1
Total: $1500
Estado: Confirmado
```

**Detalles del manejo de archivos:** la carpeta se crea sola si no existe, se guarda con
*append* (no se pisa lo anterior) y un `lock` evita que dos escrituras al mismo tiempo rompan
el archivo. El estado inicial se guarda al crear el pedido, así el historial arranca desde
"Pendiente".

---

## 7. Manejo de excepciones

| Excepción | Capa | Cuándo se lanza | Cómo se trata |
|---|---|---|---|
| `TransicionInvalidaException` | Domain | Se pide una acción que no corresponde al estado | La UI la captura, muestra el mensaje y el programa **sigue andando** |
| `TotalInvalidoException` | Domain | El total del pedido no es mayor a 0 | La UI la captura y avisa |
| `HistorialException` | DAL | Falla al escribir o leer el archivo (permisos, disco). Guarda el error original adentro | Se propaga y se informa; el pedido no cambia de estado si no se pudo guardar |
| `ArgumentNullException` | Domain / BLL / DAL | Se pasa `null` donde no corresponde | Se corta temprano con un mensaje claro |

---

## 8. Correspondencia con los criterios de evaluación

| # | Criterio (2 puntos cada uno) | Dónde se cumple |
|---|---|---|
| 1 | **Patrón State** — una clase por estado, el pedido delega, transiciones inválidas rechazadas | `IEstadoPedido`, `EstadoPedidoBase` y las 5 clases en `Estados/`; `Pedido.Confirmar/Enviar/Entregar/Cancelar` |
| 2 | **Máquina de estados** — transiciones válidas, no cancelar lo enviado, total mayor a 0 | Cada estado concreto; la validación del total está en el constructor de `Pedido` (`TotalInvalidoException`) |
| 3 | **Persistencia** — un archivo por pedido, con fecha y hora, append y manejo de errores | `RepositorioHistorialArchivo` (DAL) |
| 4 | **Salida por consola** — estado actual y acciones permitidas, errores claros | `MostrarEstado` y `Intentar` en `Program.cs` |
| 5 | **Calidad de código** — excepciones, arquitectura, comentarios | 4 capas en un solo sentido, dos excepciones de dominio, una de datos, comentarios cortos y este README |

---

## 9. Marco teórico

### Concepto de patrón de diseño

Un patrón de diseño es una **solución general y reutilizable** a un problema que aparece
seguido al diseñar software. No es código terminado: describe cómo colaboran unas clases
entre sí, y ya está probado en muchos proyectos.

### Relación entre los patrones y el desarrollo de software

Los patrones dan un **vocabulario común** y aprovechan la experiencia de otros en vez de
inventar algo nuevo cada vez. Ayudan a que el código sea más fácil de mantener y de cambiar.
El costo es que agregan abstracción: si se aplica un patrón que no hace falta, el código se
complica sin beneficio.

### Patrones respecto al nivel de abstracción

Los **patrones de diseño (GoF)** —como State, Strategy o Factory Method— trabajan entre clases
y objetos dentro de una aplicación. Los **de arquitectura** —como capas o MVC— organizan el
sistema completo. Acá se usan los dos niveles: el patrón de **capas** ordena todo el sistema y
el patrón **State** resuelve, dentro del dominio, el comportamiento variable del pedido.

### Análisis y detección del problema

Primero hay que **detectar el problema**, no partir del patrón. En este ejercicio la señal fue
esta: un objeto que se comporta distinto según su estado interno y varios `if`/`switch` que
preguntan "¿en qué estado estoy?" antes de cada acción. Esa es la señal típica de State. Si el
problema hubiera sido "avisar a varios objetos cuando algo cambia", el patrón era Observer; si
hubiera sido "elegir entre varios algoritmos", Strategy.

### El producto y el proceso del desarrollo de software

El **producto** es lo que se entrega (código, datos, documentación). El **proceso** son las
actividades para producirlo: relevamiento, análisis, diseño, construcción, pruebas y
mantenimiento. Los patrones ayudan a los dos: ordenan el proceso porque dan una guía de diseño
ya probada, y mejoran el producto porque dejan una estructura más clara.

### Gestión del riesgo

Gestionar el riesgo es **identificar** qué puede salir mal, **evaluar** su impacto y **decidir**
qué hacer. En esta solución:

- **Que se agregue un estado nuevo** (por ejemplo `Devuelto`): se resuelve con State, creando
  una clase nueva sin tocar las demás.
- **Que se intente una acción inválida**: se rechaza con una excepción propia, en vez de dejar
  el pedido en un estado inconsistente.
- **Que falle el guardado del historial**: el cambio de estado no queda a medias y el error se
  informa con la ruta del archivo.
- **Datos inválidos**: el total se valida en el dominio, en un solo lugar.

---

## 10. Estructura del repositorio

```
Practica-PP3-Patron-State/
├── PedidoApp.sln
├── Enunciado.md
├── README.md
├── .gitignore
├── .gitattributes
├── Historiales/                          <- se genera al ejecutar
│   └── .gitkeep
│
├── PedidoApp.Domain/                     <- capa de dominio (sin dependencias)
│   ├── Pedido.cs                         <- el contexto
│   ├── IEstadoPedido.cs                  <- la interfaz de los estados
│   ├── EstadoPedidoBase.cs               <- acciones que no van = error
│   ├── ResumenPedido.cs                  <- foto del pedido
│   ├── Estados/
│   │   ├── EstadoPendiente.cs
│   │   ├── EstadoConfirmado.cs
│   │   ├── EstadoEnviado.cs
│   │   ├── EstadoEntregado.cs
│   │   └── EstadoCancelado.cs
│   └── Exceptions/
│       ├── TransicionInvalidaException.cs
│       └── TotalInvalidoException.cs
│
├── PedidoApp.DAL/                        <- capa de datos
│   ├── IRepositorioHistorial.cs
│   ├── RepositorioHistorialArchivo.cs
│   ├── Excepciones/
│   │   └── HistorialException.cs
│   └── Formato/
│       ├── IFormateadorHistorial.cs
│       ├── FormateadorTextoPlano.cs
│       └── FormateadorDetallado.cs
│
├── PedidoApp.BLL/                        <- capa de negocio
│   ├── ServicioPedido.cs
│   └── FabricaPedido.cs
│
└── PedidoApp.UI/                         <- capa de presentación (el ejecutable)
    └── Program.cs
```

---

## 11. Salida esperada

Fragmento de la demostración:

```
Paso 2: lo envio.

Pedido 1 - Total $1500
Estado: Enviado
Acciones permitidas: entregar

Paso 4: intento cancelarlo, pero ya esta entregado.
Se controlo el error: No se puede cancelar un pedido en estado Entregado
```

Y el resumen final:

```
=== Resumen ===
Estado final del pedido 1: Entregado
Cambios guardados: 4
Fin del programa.
```

---

## 12. Menú

| Opción | Acción |
|---|---|
| 1 | Confirmar |
| 2 | Enviar |
| 3 | Entregar |
| 4 | Cancelar |
| 5 | Ver el estado actual |
| 6 | Ver el historial |
| 0 | Salir |
