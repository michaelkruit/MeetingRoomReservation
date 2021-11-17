// Button to add new attendees
const attendeeBtn = document.getElementById('add-attendee-btn');
// Container that contains the attendees
const attendeesContainter = document.getElementById('attendees-container');

// Add click event listener to attendeeBtn that executes addAttendee();
attendeeBtn.addEventListener('click', addAttendee);

// Add an attendee to attendees list
function addAttendee() {
    // Create div that will contain an attendee input
    let div = document.createElement('div');
    // Attendee input with necessary attributes
    let newInput = `<div class="input-group mb-3">
                        <input type="text" class="form-control" name="Attendees[]" value="" />
                        <button type="button" class="btn btn-sm btn-danger" onclick="removeAttendee(this)">Remove</button>
                    </div>`;
    // Add attendee input to div container
    div.innerHTML = newInput;
    // Append div to attendees container
    attendeesContainter.appendChild(div);
}

// Remove an existing attendee from list
function removeAttendee(elem) {
    // Find parent container of attendee
    const divToRemove = elem.parentNode;
    // Remove container from DOM
    divToRemove.remove();
}