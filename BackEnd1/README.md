# Backend - Reserva Senac

API REST em ASP.NET Core 9 criada em C# para o sistema de reservas de salas.

## Configuracao

1. Execute o arquivo `Exercicio-1/schema.sql` no MySQL Workbench.
2. Altere `appsettings.json` e informe a senha do usuario MySQL:

```json
"ConnectionStrings": {
  "MySql": "Server=localhost;Port=3306;Database=reserva_salas;User=root;Password=SUA_SENHA;"
}
```

3. Inicie a API:

```powershell
cd C:\workspace\Exercicios\1\BackEnd1
dotnet run --urls http://localhost:5080
```

4. Acesse o OpenAPI em `http://localhost:5080/openapi/v1.json`.

## Endpoints

### Salas

- `GET /api/salas`
- `GET /api/salas/{id}`
- `POST /api/salas`
- `PUT /api/salas/{id}`
- `DELETE /api/salas/{id}`

Filtros opcionais em `GET /api/salas`: `location` e `capacity`.

### Reservas

- `GET /api/reservas`
- `GET /api/reservas/{id}`
- `POST /api/reservas`
- `PUT /api/reservas/{id}`
- `PATCH /api/reservas/{id}/cancelar`
- `DELETE /api/reservas/{id}`

Filtros opcionais em `GET /api/reservas`: `date`, `roomId`, `status` e `search`.

### Usuarios

- `GET /api/usuarios`
- `GET /api/usuarios/{id}`

## Formato de reserva

```json
{
  "idSala": 1,
  "idResponsavel": 1,
  "objetivo": "Planejamento comercial",
  "dataReserva": "2026-11-17",
  "horarioInicio": "09:00:00",
  "horarioFim": "10:30:00",
  "status": "confirmada",
  "cor": null
}
```

O backend valida campos obrigatorios e conflito de horario antes de salvar. O banco tambem possui gatilhos que impedem conflitos em insercoes e alteracoes.
