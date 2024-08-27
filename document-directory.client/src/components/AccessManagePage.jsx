import React, {useEffect, useState} from 'react';
import dayjs from 'dayjs';
import { useNavigate } from 'react-router-dom';
import Cookie from 'js-cookie';
import {Button, Table, Select} from 'antd';


function AccessManagePage({setIsModalVisible, chosenNode, setChosenNode}) {
    const [usersWithAccess, setUsersWithAccess] = useState([]);
    const [groupsWithAccess, setGroupsWithAccess] = useState([]);
    const [allUserGroups, setAllUserGroups] = useState([]);
    const [allUsers, setAllUsers] = useState([]);
    // const [chosenNode, setChosenNode] = useState([]);
    const [selectedUsers, setSelectedUsers] = useState([]);
    const [selectedGroups, setSelectedGroups] = useState([]);

    const navigate = useNavigate();

    async function setAccess() {
        let selectedUsersId = selectedUsers.flat();
        let selectedGroupsId = selectedGroups.flat();
        if (selectedUsers[0] !== null && typeof selectedUsers[0] === "object")
        {
            selectedUsersId = selectedUsers.map((user) => user.value);
        }
        if (selectedGroups[0] !== null && typeof selectedGroups[0] === "object")
        {
            selectedGroupsId = selectedGroups.map((user) => user.value);
        }
        const response = await fetch('https://localhost:7018/api/nodeaccess', {
            method: 'PUT',
            headers: new Headers({"Content-Type": "application/json"}),
            credentials: 'include',
            body: JSON.stringify({
                nodeId: chosenNode.id,
                groupsId: selectedGroupsId,
                usersId: selectedUsersId
            })
        });
        if (response.status == 200)
        {
            const json = await response.json();
            setIsModalVisible(false);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test');
        }
    }

    async function fetchAccessData(node) {
        setChosenNode(node);
        await getUsersWithAccess(node);
        await getGroupsWithAccess(node);
        await getAllUsers();
        await getAllUserGroups();
        setIsModalVisible(true);
    }

    async function getAllUserGroups() {
        const response = await fetch('https://localhost:7018/api/groups/user', {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await response.json();
            setAllUserGroups(json);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function getGroupsWithAccess(node) {
        const response = await fetch(`https://localhost:7018/api/groups/with-access-to-node/${node.id}`, {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await response.json();
            setGroupsWithAccess(json);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function getAllUsers() {
        const response = await fetch('https://localhost:7018/api/users/exclude-admins', {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await response.json();
            setAllUsers(json);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function getUsersWithAccess(node) {
        const response = await fetch(`https://localhost:7018/api/users/with-access-to-node/${node.id}`, {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await response.json();
            setUsersWithAccess(json);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    
    useEffect(() => {
        fetchAccessData(chosenNode);
    }, [])

    useEffect(() => {
        setSelectedUsers(usersWithAccess.map((user) => ({label: user.login, value: user.id})));
    }, [usersWithAccess])

    useEffect(() => {
        setSelectedGroups(groupsWithAccess.map((group) => ({label: group.name, value: group.id})));
    }, [groupsWithAccess])

    return (
        <div>
            {Object.keys(chosenNode).length !== 0 && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div>
                            <h3>Пользователи, обладающие доступом</h3>
                            <Select
                                mode="multiple"
                                allowClear
                                style={{
                                    width: '80%',
                                    marginBottom: '40px'
                                }}
                                placeholder="Выберите пользователей, которым хотите дать доступ"
                                onChange={setSelectedUsers}
                                value={selectedUsers}
                                options={allUsers.map((user) => ({label: user.login, value: user.id}))}
                            />
                            <br />
                            <h3>Группы, обладающие доступом</h3>
                            <Select
                                mode="multiple"
                                allowClear
                                style={{
                                    width: '80%',
                                }}
                                placeholder="Выберите группы, которым хотите дать доступ"
                                options={allUserGroups.map((group) => ({label: group.name, value: group.id}))}
                                onChange={setSelectedGroups}
                                value={selectedGroups}
                            />
                            <div style={{display: 'flex', justifyContent: 'space-around'}}>
                                <Button size='small' onClick={setAccess} style={{width: "100px", marginTop: '10px'}}>Сохранить</Button>
                                <Button size='small' onClick={() => setIsModalVisible(false)} style={{width: "100px", marginTop: '10px'}}>Отменить</Button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    )
}


export default AccessManagePage;
