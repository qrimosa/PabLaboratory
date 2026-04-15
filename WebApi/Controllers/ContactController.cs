using AppCore.Authorization;
using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AppCore.Dto;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController(IPersonService service) : ControllerBase
{
    // -------------------------------------------------------------------------
    // PERSON ENDPOINTS
    // -------------------------------------------------------------------------

    // 1. GET ALL (Paginated)
    [HttpGet]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetAll(int page = 1, int size = 10)
    {
        var result = await service.FindAllPeoplePaged(page, size);
        return Ok(result);
    }
    

    // 2. GET BY ID (Crucial: Fetches the specific person you just created)
    [HttpGet("{contactId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid contactId)
    {
        try
        {
            var person = await service.GetPerson(contactId);
            return Ok(person);
        }
        catch (AppCore.Exceptions.ContactNotFoundException)
        {
            return NotFound($"Person with id {contactId} not found.");
        }
    }

    // 3. CREATE PERSON (Fixes your 405 error!)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
    {
        var person = await service.AddPerson(dto);
        
        // This returns a 201 Created and tells the client where to find the new resource
        return CreatedAtAction(nameof(Get), new { contactId = person.Id }, person);
    }

    // 4. UPDATE PERSON
    [HttpPut("{contactId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid contactId, [FromBody] UpdatePersonDto dto)
    {
        var updatedPerson = await service.UpdatePerson(contactId, dto);
        
        if (updatedPerson == null)
            return NotFound($"Person with id {contactId} not found.");

        return Ok(updatedPerson);
    }

    // 5. DELETE PERSON
    [HttpDelete("{contactId:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.AdminOnly))]
    public async Task<IActionResult> Delete([FromRoute] Guid contactId)
    {
        await service.DeletePerson(contactId);
        return NoContent();
    }


    // -------------------------------------------------------------------------
    // NOTES ENDPOINTS
    // -------------------------------------------------------------------------

    [HttpGet("{contactId:guid}/notes")]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        try
        {
            var person = await service.GetPerson(contactId);
            return Ok(person.Notes);
        }
        catch (AppCore.Exceptions.ContactNotFoundException)
        {
            return NotFound($"Person with id {contactId} not found.");
        }
    }

    [HttpPost("{contactId:guid}/notes")]
    public async Task<IActionResult> AddNote([FromRoute] Guid contactId, [FromBody] CreateNoteDto dto)
    {
        var note = await service.AddNoteToPerson(contactId, dto);
        return CreatedAtAction(nameof(GetNotes), new { contactId }, note);
    }

    [HttpDelete("{contactId:guid}/notes/{noteId:guid}")]
    public async Task<IActionResult> DeleteNote([FromRoute] Guid contactId, [FromRoute] Guid noteId)
    {
        await service.DeleteNoteFromPerson(contactId, noteId);
        return NoContent();
    }
}