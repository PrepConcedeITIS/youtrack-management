import React from 'react';
import axios from 'axios';

//const callApi = axios.post('http://localhost:3/gateway/assign/');
const callApi = () => axios.get('https://eeb3-178-204-152-215.ngrok.io/MockTrainData/csv');

const AssigneeWidget: React.FC = () => (
  <button onClick={() => callApi()}>{'Click and call'}</button>
);

export default AssigneeWidget;
