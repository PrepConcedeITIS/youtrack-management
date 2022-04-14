import React, {useEffect, useState} from 'react';
import axios from 'axios';
import {loadAgiles} from "./resources";

//const callApi = axios.post('http://localhost:3/gateway/assign/');
const callApi = () => console.log(123); //axios.post('https://3c2e-178-204-152-215.ngrok.io/ManageAssignees/assign/ins-sprint');



const AssigneeWidget: React.FC = ({dashboardApi}) => {
	const fetchYouTrack = async (url, params) => {
		console.log(dashboardApi);
		const {selectedYouTrack} = this.state;
		return await dashboardApi.fetch(selectedYouTrack.id, url, params);
	};
	console.log(window.location);
	const host = 'https://localhost:14000/';
	const assignCurrentSprint = () => {
		return axios.post(`${host}/AssignSprint`, {'sprintName': 'Sprint 1', 'projectShortName': 'AVG'});
	}

	const resetAssigneesInCurrentSprint = () => {
		return axios.delete(`${host}/unAssignSprint`, {'projectShortName': 'AVG'});
	}
	// const [agiles, setAgiles] = useState();
	// useEffect(() => {
	// 	const fetch = async () => {
	// 		const agilesResponse = await loadAgiles(fetchYouTrack);
	// 		console.log(agilesResponse);
	// 	}
	// }, []);
	return (
		<div>
			<button type="button" onClick={assignLastSprint}>{'Click and call'}</button>
		</div>
	);
};

export default AssigneeWidget;
