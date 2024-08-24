import React, {useEffect, useState} from 'react';
import dayjs from 'dayjs';
import {Transfer, Button, Typography, Table} from 'antd';


function AccessManagePage() {
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [availableNodes, setAvailableNodes] = useState([]);
    const [usersWithoutAdmins, setUsersWithoutAdmins] = useState([]);
    const [usersWithAccess, setUsersWithAccess] = useState([]);

    function handleClick() {
        setIsModalVisible(true);
    }

    async function getUsersExcludeSelfAndAdmins() {
        const response = await fetch("https://localhost:7018/api/users/exclude-self-and-admins", {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await resonse.json();
            setUsersWithoutAdmins(json);
        }
    }

    async function getUsersWithAccess() {
        const response = await fetch("https://localhost:7018/api/users/access-to-node", {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await resonse.json();
            setUsersWithAccess(json);
        }
    }

    async function getAvailableNodes() {
        const response = await fetch("https://localhost:7018/api/folders/access", {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
        }
    }

    

    const columns = [
        {
            title: 'Название',
            dataIndex: 'name',
            sorter: (a, b) => a.name.length - b.name.length,
            width: '50%',
        },
        {
            title: 'Тип',
            dataIndex: 'type',
            // render: (type) => type == "Directory" ? "Директория" : "Документ",
            onFilter: (value, record) => record.type == value,
            filters: [
                {
                    text: 'Документ',
                    value: 'Document',
                },
                {
                    text: 'Директория',
                    value: 'Directory',
                },
            ],
            width: '8%',
        },
        {
            title: 'Дата создания',
            dataIndex: 'createdAt',
            width: '12%'
        },
        {
            title: 'Дата активности',
            dataIndex: 'activityEnd',
            width: '10%'
        },
        {
            title: 'Действие',
            width: '20%',
            render: () => (
                <a onClick={handleClick}>Управление доступом</a>
            )
        } 
    ];

    useEffect(() => {
        getAvailableNodes();
        getUsersExcludeSelfAndAdmins();
        getUsersWithAccess();
    }, [])

    return (
        <div>
            {isModalVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div>
                            <h3>Выберите пользователей, которым хотите предоставить доступ</h3>
                            <Transfer
                                // dataSource={availableNodes}
                                titles={['С доступом', 'Без доступа']}
                                targetKeys={usersWithAccess}
                                selectedKeys={usersWithoutAdmins}
                                render={(node) => node.name}
                                locale={
                                    {
                                        itemUnit: '',
                                        itemsUnit: '',
                                        notFoundContent: 'Список пуст'
                                    }
                                }
                                listStyle={{
                                    width: '500px',
                                    height: '300px',
                                    textAlign: 'left'
                                }}
                            />
                            <br />
                            <h3>Выберите группы пользователей, которым хотите предоставить доступ</h3>
                            <Transfer
                                dataSource={availableNodes}
                                titles={['С доступом', 'Без доступа']}
                                targetKeys={availableNodes}
                                selectedKeys={[]}
                                render={(node) => node.name}
                                locale={
                                    {
                                        itemUnit: '',
                                        itemsUnit: '',
                                        notFoundContent: 'Список пуст'
                                    }
                                }
                                listStyle={{
                                    width: '500px',
                                    height: '300px',
                                    textAlign: 'left'
                                }}
                            />
                            <div style={{display: 'flex', justifyContent: 'space-around'}}>
                                <Button size='small' onClick={() => setIsModalVisible(false)} style={{width: "100px", marginTop: '10px'}}>Сохранить</Button>
                                <Button size='small' onClick={() => setIsModalVisible(false)} style={{width: "100px", marginTop: '10px'}}>Отменить</Button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
            <Table columns={columns} dataSource={availableNodes} rowKey={(node) => node.id}/>
        </div>
    )
}


export default AccessManagePage;
