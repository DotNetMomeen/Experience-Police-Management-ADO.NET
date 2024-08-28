<h2>👮‍♂️ Experienced Police Management System (EPMS) 👮‍♀️</h2>

<p>The Experienced Police Management System (EPMS) is designed to efficiently manage and track police personnel, their designations, and work experience. This database system ensures accurate record-keeping and ease of access to vital information.</p>

<h3>🔍 System Overview</h3>
<p>EPMS organizes data into key tables, including <code>Designation</code>, <code>Police</code>, and <code>Experience</code>. The system provides a comprehensive approach to managing police officers' records, their designations, and their years of service.</p>

<h3>🛡️ Designation Management</h3>
<p>The <code>Designation</code> table maintains a list of police ranks, such as Constable and Captain. Each designation has a unique <code>DesignationId</code> and a title, enabling efficient categorization of police roles.</p>

<h3>👮‍♂️ Police Records</h3>
<p>The <code>Police</code> table stores detailed information about each officer, including their <code>PoliceId</code>, code, name, date of birth, gender, and designation. It also tracks whether the officer is permanent and includes an optional image path for visual identification.</p>

<h3>📅 Experience Tracking</h3>
<p>The <code>Experience</code> table logs each officer's work experience, noting the police station, years worked, and linking back to the <code>Police</code> table. This helps in maintaining a record of service duration and locations.</p>

<h3>🛠️ Stored Procedures</h3>
<ul>
    <li><strong><code>PoliceExperienceAddAndEdit</code>:</strong> Adds or updates police experience records based on the provided <code>ExperienceId</code>.</li>
    <li><strong><code>PoliceAddOrEdit</code>:</strong> Handles insertion or updating of police details.</li>
    <li><strong><code>PoliceExperienceDelete</code>:</strong> Deletes police records and associated experience details.</li>
    <li><strong><code>ExperienceDelete</code>:</strong> Deletes specific experience records.</li>
    <li><strong><code>ViewAllPolice</code>:</strong> Retrieves all police records with total experience and designation details.</li>
    <li><strong><code>ViewPoliceByPoliceId</code>:</strong> Fetches detailed records of a specific police officer.</li>
</ul>

<h3>🎨 System Design Highlights</h3>
<ul>
    <li><strong>Structured Data Management:</strong> Clear table relationships ensure data integrity and organization.</li>
    <li><strong>Efficient Data Access:</strong> Stored procedures streamline operations for adding, updating, and deleting records.</li>
    <li><strong>Comprehensive Reporting:</strong> Detailed views and reports provide complete insights into police records and experience.</li>
</ul>

<h3>🚀 Why EPMS?</h3>
<p>EPMS offers a reliable and scalable solution for managing police records. Its structured design and comprehensive features enhance data management, improve operational efficiency, and support effective decision-making in law enforcement.</p>
