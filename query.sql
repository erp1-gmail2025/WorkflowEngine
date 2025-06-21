SELECT * FROM Processes

SELECT s.*, st.Name AS StateTypeName, st.[Key] AS StateTypeKey FROM States s LEFT JOIN StateTypes st ON s.StateTypeID = st.StateTypeID 

SELECT * FROM Requests

SELECT t.TransitionID AS TransitionId
	, p.Name AS ProcessName
	, s.Description AS CurrentStateName
	, st.Name AS CurrentStateTypeName 
	, s2.Description AS NextStateName
	, st2.Name As NextStateTypeName
	FROM Transitions t 
	LEFT JOIN States s ON t.CurrentStateID = s.StateID
	LEFT JOIN States s2 ON t.NextStateID = s2.StateID
	LEFT JOIN StateTypes st ON st.StateTypeID = s.StateTypeID
	LEFT JOIN StateTypes st2 ON st2.StateTypeID = s2.StateTypeID
	LEFT JOIN Processes p ON p.ProcessID = s.ProcessID


SELECT r.RequestID, r.ProcessId, r.RequestID, s.Description FROM Requests r
	LEFT JOIN States s ON r.CurrentStateID = s.StateID
	LEFT JOIN StateTypes st ON st.StateTypeID = s.StateTypeID

SELECT * FROM WorkflowActions
