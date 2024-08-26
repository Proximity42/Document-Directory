import React, {useEffect, useState} from 'react';
import dayjs from 'dayjs';
import {Button, Table, Select} from 'antd';


function AccessManagePage() {
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [availableNodes, setAvailableNodes] = useState([]);
    const [usersWithoutAccess, setUsersWithoutAdmins] = useState([]);
    const [usersWithAccess, setUsersWithAccess] = useState([]);
    const [groupsWithAccess, setGroupsWithAccess] = useState([]);
    const [options, setOptions] = useState([]);

    async function editAccess(node) {
        await getUsersWithAccess(node);
        let optionsLocal = usersWithAccess.map((user) => {return {title: user.login, value: user}});
        console.log(optionsLocal);
        setOptions(optionsLocal);
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

    async function getUsersWithAccess(node) {
        const response = await fetch(`https://localhost:7018/api/users/with-access-to-node/${node.id}`, {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            const json = await response.json();
            setUsersWithAccess(json);
        }
    }

    async function getAvailableNodes() {
        const response = await fetch("https://localhost:7018/api/NodeHierarchy", {
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
            render: (_, record) => (
                <a onClick={() => editAccess(record)}>Управление доступом</a>
            )
        } 
    ];

    useEffect(() => {
        getAvailableNodes();
        // getUsersExcludeSelfAndAdmins();
        // getUsersWithAccess();
    }, [])

    return (
        <div>
            {isModalVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div>
                            <h3>Пользователи, обладающие доступом</h3>
                            {/* <Transfer
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
                            /> */}
                            <Select
                                mode="multiple"
                                allowClear
                                style={{
                                    width: '100%',
                                }}
                                placeholder="Выберите пользователей"
                                defaultValue={usersWithAccess}
                                // onChange={handleChange}
                                options={options}
                            />
                            <br />
                            <h3>Группы, обладающие доступом</h3>
                            {/* <Transfer
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
                            /> */}
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
