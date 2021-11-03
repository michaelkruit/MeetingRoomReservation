const meetingRoomId = document.getElementById("MeetingRoomId").value;
const meetingsUiList = document.getElementById('meetings');
const meetingTimeHeader = document.getElementById('meeting-time');
const attendingCompanyHeader = document.getElementById('attending-company');
let meetings = [];

window.setInterval(function () {
    setTime();
    getMeetings();
    checkMeetings();
    appendToUiList();
}, 3000);

function setTime() {
    const today = new Date();
    const currentTime = `${today.getHours()}:${today.getMinutes()}`;
    const timeSpan = document.getElementById('time');
    timeSpan.innerHTML = currentTime;
}

function getMeetings() {
    fetch(`/Display/GetMeetings?meetingRoomId=${meetingRoomId}`)
        .then(response => response.json())
        .then(data => data.forEach(x => meetings.push(x)));
}

function checkMeetings() {
    if (meetings.length === 0) {
        return;
    }
    const currentDate = new Date();
    const firstMeeting = meetings[0];
    const firstMeetingDate = new Date(firstMeeting.startDateTime);
    if (firstMeetingDate.getTime() <= currentDate.getTime()) { // Meeting busy
        toggleBusyState(meetings[0]);
    }
    else if (getDiffInMinutes(currentDate, firstMeetingDate) < 15) { // Meeting starts within 15 minutes
        toggleAlmostState(meetings[0]);
    }
    else { // No meeting within 15 minutes
        toggleAvailableState();
    }
}

function appendToUiList() {
    meetingsUiList.innerHTML = '';
    meetings.forEach(meeting => {
        let li = document.createElement('li');
        li.appendChild(document.createTextNode(`${meeting.attendingCompany} ${meeting.startShortTimeString} - ${meeting.endShortTimeString}`));
        li.setAttribute('id', meeting.id);
        meetingsUiList.appendChild(li);
    });
    meetings = [];
}

function toggleBusyState(meeting) {
    meetingTimeHeader.innerHTML = `${meeting.startShortTimeString} - ${meeting.endShortTimeString}`;
    attendingCompanyHeader.innerHTML = meeting.attendingCompany;
    document.body.style.backgroundColor = 'red';
    meetings.shift();
}

function toggleAlmostState(meeting) {
    meetingTimeHeader.innerHTML = `${meeting.startShortTimeString} - ${meeting.endShortTimeString}`;
    attendingCompanyHeader.innerHTML = meeting.attendingCompany;
    document.body.style.backgroundColor = 'orange';
}

function toggleAvailableState() {
    meetingTimeHeader.innerHTML = '';
    attendingCompanyHeader.innerHTML = '';
    document.body.style.backgroundColor = 'green';
}

function getDiffInMinutes(date1, date2) {
    var diff = (date2.getTime() - date1.getTime()) / 1000;
    diff /= 60;
    return Math.abs(Math.round(diff));
}