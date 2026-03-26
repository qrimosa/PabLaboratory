using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AppCore.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int size = 10)
    {
        var result = await service.FindAllPeoplePaged(page, size);
        return Ok(result);
    }
    [HttpPost("{contactId:guid}/notes")]
    public async Task<IActionResult> AddNote([FromRoute] Guid contactId, [FromBody] CreateNoteDto dto)
    {
        var note = await service.AddNoteToPerson(contactId, dto);
        return CreatedAtAction(nameof(GetNotes), new { contactId }, note);
    }

    [HttpGet("{contactId:guid}/notes")]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        var person = await service.GetPerson(contactId);
        return Ok(person.Notes);
    }

    [HttpDelete("{contactId:guid}/notes/{noteId:guid}")]
    public async Task<IActionResult> DeleteNote(Guid contactId, Guid noteId)
    {
        await service.DeleteNoteFromPerson(contactId, noteId);
        return NoContent();
    }
}