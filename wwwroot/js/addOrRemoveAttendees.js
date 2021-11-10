const attendeeBtn = document.getElementById('add-attendee-btn');
const attendeesContainter = document.getElementById('attendees-container');

attendeeBtn.addEventListener('click', addAttendee);

function addAttendee() {
    let div = document.createElement('div');
    let newInput = `<div class="input-group mb-3">
                        <input type="text" class="form-control" name="Attendees[]" value="" />
                        <button type="button" class="btn btn-sm btn-danger" onclick="removeAttendee(this)">Remove</button>
                    </div>`;
    div.innerHTML = newInput;
    attendeesContainter.appendChild(div);
}

function removeAttendee(elem) {
    const divToRemove = elem.parentNode;
    divToRemove.remove();
}