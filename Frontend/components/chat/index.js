import React from "react";

export default class Chat extends React.Component {

	constructor(props) {
		super(props);

		this.state = {
			messages: []
		};

		this.addressInput = React.createRef();
		this.messageInput = React.createRef();

		this.openClickHandler = this.openClickHandler.bind(this);
		this.closeClickHandler = this.closeClickHandler.bind(this);
		this.sendClickHandler = this.sendClickHandler.bind(this);

		this.token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjgyNDU0NzEzLWQ2YzYtNGUyMC04MmRmLTM0MjkwY2RkODdhOSIsImV4cCI6MTU1MTAxNzQ0NH0.VJWiOlK_by3mmhqYJlDjm5zLs4eYj5GIjajdBtfDwz8";
	}

	openClickHandler() {
		var address = this.addressInput.current.value;

		this.socket = new WebSocket(address);

		this.socket.onopen = (event) => {
			console.log(event);
		};

		this.socket.onclose = (event) => {
			console.log(event);
		};

		this.socket.onerror = (event) => {
			console.log(event);
		};

		this.socket.onmessage = (event) => {
			var message = JSON.parse(event.data);
			
			this.setState((state) => {
				return { messages: [...state.messages, message] };
			});
		};
	}

	closeClickHandler() {
		this.socket.close();
	}

	sendClickHandler() {
		fetch('http://localhost:5000/api/conversations/public/messages', {
			method: 'POST',
			headers: {
				'content-type': 'application/json',
				'authorization': 'Bearer ' + this.token
			},
			body: JSON.stringify({
				content: this.messageInput.current.value
			})
		});
	}

	render() {
		return (
			<React.Fragment>
				<p>{this.state.status}</p>

				<div>
					<label htmlFor="connectionUrl">WebSocket Server URL:</label>
					<input ref={this.addressInput} defaultValue={"ws://localhost:5000/messages?token=" + this.token} />
					<button onClick={this.openClickHandler}>Open Socket</button>
				</div>
				<p></p>
				<div>
					<label htmlFor="sendMessage">Message to send:</label>
					<input ref={this.messageInput} defaultValue="Привет!" />
					<button onClick={this.sendClickHandler}>Send Message</button>
					<button onClick={this.closeClickHandler}>Close Socket</button>
				</div>

				<h2>Log</h2>
				<table>
					<thead>
						<tr>
							<td>From</td>
							<td>To</td>
							<td>Date</td>
							<td>Data</td>
						</tr>
					</thead>
					<tbody>
						{
							this.state.messages.map(message => 
								(
									<tr key={message.Id}>
										<td>{message.User}</td>
										<td>{message.ConversationId}</td>
										<td>{message.Timestamp}</td>
										<td>{message.Content}</td>
									</tr>
								)
							)
						}
					</tbody>
				</table>

			</React.Fragment>
		);
	}
}