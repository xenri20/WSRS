import { evaluateClassification } from  '../utils/evaluateClassification.js';

// Global elements
const studentModalBody = document.querySelector('#studentModal .modal-body')

const fetchStudentData = async (number) => {
    const url = `/api/Students/?studentNumber=${number}`;

    try {
        const request = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        });

        const response = await request.json();

        return response; 
    } catch (error) {
        console.error(new Error(error.message));
        alert('Something went wrong getting the student data');
    }
};

const clearModalBody = () => studentModalBody.textContent = '';

// Regular table component
const constructRegularTable = (data) => {

    if (data.length === 0) {
        const empty = document.createElement('div');

        empty.textContent = 'No records to show';
        return empty;
    }

    const offenseTable = document.createElement('table');
    const offenseTableHeader = document.createElement('thead');
    const offenseTableBody = document.createElement('tbody');

    const headers = ['Commission Date', 'Classification', 'Nature', 'Sanction'];
    const headerRow = document.createElement('tr');
    headers.forEach(headerText => {
        const header = document.createElement('th');
        header.textContent = headerText;
        headerRow.appendChild(header);
    });
    offenseTableHeader.appendChild(headerRow);

    data.forEach(report => {
        const row = document.createElement('tr');

        const dateCell = document.createElement('td');
        const classificationCell = document.createElement('td');        
        const natureCell = document.createElement('td');
        const sanctionCell = document.createElement('td');

        classificationCell.textContent = evaluateClassification(report.offense.classification);
        natureCell.textContent = report.offense.nature;
        dateCell.textContent = new Date(report.commissionDate).toLocaleDateString();
        sanctionCell.textContent = report.sanction;

        row.appendChild(dateCell);
        row.appendChild(classificationCell);
        row.appendChild(natureCell);
        row.appendChild(sanctionCell);
        offenseTableBody.appendChild(row);
    });

    offenseTable.appendChild(offenseTableHeader);
    offenseTable.appendChild(offenseTableBody);
    return offenseTable;
};

const constructTrafficTable = (data) => {

    if (data.length === 0) {
        const empty = document.createElement('div');

        empty.textContent = 'No records to show';
        return empty;
    }
    
    const offenseTable = document.createElement('table');
    const offenseTableHeader = document.createElement('thead');
    const offenseTableBody = document.createElement('tbody');

    const headers = ['Commission Date', 'Classification', 'Nature', 'Plate Number', 'Remarks'];
    const headerRow = document.createElement('tr');
    headers.forEach(headerText => {
        const header = document.createElement('th');
        header.textContent = headerText;
        headerRow.appendChild(header);
    });
    offenseTableHeader.appendChild(headerRow);

    data.forEach(report => {
        const row = document.createElement('tr');

        const dateCell = document.createElement('td');
        const classificationCell = document.createElement('td');        
        const natureCell = document.createElement('td');
        const plateNumberCell = document.createElement('td');        
        const remarksCell = document.createElement('td');

        classificationCell.textContent = evaluateClassification(report.offense.classification);
        natureCell.textContent = report.offense.nature;
        plateNumberCell.textContent = report.plateNumber;
        dateCell.textContent = new Date(report.commissionDate).toLocaleDateString();
        remarksCell.textContent = report.remarks;

        row.appendChild(dateCell);
        row.appendChild(classificationCell);
        row.appendChild(natureCell);
        row.appendChild(plateNumberCell);
        row.appendChild(remarksCell);
        offenseTableBody.appendChild(row);
    });

    offenseTable.appendChild(offenseTableHeader);
    offenseTable.appendChild(offenseTableBody);
    return offenseTable;
};

const constructHeading = (data) => {
    const heading = document.createElement('div');
    const title = document.createElement('h2');
    const subtitle = document.createElement('p');

    title.textContent = `${data.firstName} ${data.lastName}`;
    subtitle.textContent = data.studentNumber;

    heading.appendChild(title);
    heading.appendChild(subtitle);

    return heading;
}

// Attaches itself to the document so that it can be used anywhere where it's needed
// e.g. when clicking on an element with a student number
document.addEventListener('click', async e => {
    if (e.target.type === 'button' && e.target.hasAttribute('data-student')) {
        const studentData = await fetchStudentData(e.target.getAttribute('data-student'));

        clearModalBody();

        studentModalBody.appendChild(constructHeading(studentData));

        // sort regular records by descending
        studentData.reportsEncoded.sort((a, b) => {
            return new Date(b.commissionDate) - new Date(a.commissionDate);
        });

        let tableTitle = document.createElement('h3');
        tableTitle.textContent = 'Regular Violations';
        studentModalBody.appendChild(tableTitle);
        studentModalBody.appendChild(constructRegularTable(studentData.reportsEncoded));

        // sort traffic records by descending
        studentData.trafficReportsEncoded.sort((a, b) => {
            return new Date(b.commissionDate) - new Date(a.commissionDate);
        });

        tableTitle = document.createElement('h3');
        tableTitle.classList.add('mt-3');
        tableTitle.textContent = 'Traffic Violations';
        studentModalBody.appendChild(tableTitle);
        studentModalBody.appendChild(constructTrafficTable(studentData.trafficReportsEncoded));
    }
});


