import { useEffect, useState } from 'react';
import { List, Modal, Button, Input, message, Form, Transfer } from 'antd';
import { UsergroupAddOutlined } from '@ant-design/icons';


function ListGroupComponent() {

    const [listGroup, setListGroup] = useState([]);
    const [item, setItem] = useState({});

    const [isShowModalRename, setIsShowModalRename] = useState(false);
    const [isShowModalDelete, setIsShowModalDelete] = useState(false);
    const [isShowModalParticipants, setIsShowModalParticipants] = useState(false);
    const [isShowModalCreate, setIsShowModalCreate] = useState(false);

    const [participantsGroup, setParticipantsGroup] = useState([]);
    const [otherUser, setOtherUser] = useState([]);

    const [mockData, setMockData] = useState([]);
    const [targetKeys, setTargetKeys] = useState([]);
    const [mockDataCreate, setMockDataCreate] = useState([]);
    const [targetKeysCreate, setTargetKeysCreate] = useState([]);

    const [messageApi, contextHolder] = message.useMessage();

 

    function showModalRename(item) {
        setItem(item);
        setIsShowModalRename(true)
    }

    

    function showModalDelete(item) {
        setItem(item);
        setIsShowModalDelete(true)
    }

    function showModalParticipants(item) {
        getPartisipantsAndOtherUser(item.id);
        setItem(item);
        setIsShowModalParticipants(true);
        
        
        //setTitle('Проверка');
    }

    function showModalCreate() {
        getAll();
        setIsShowModalCreate(true);
    }

    const handleCancel = () => {
        setIsShowModalRename(false);
        setIsShowModalDelete(false);
        setIsShowModalParticipants(false);
        setIsShowModalCreate(false);
    };

    async function getAll() {
        let tempTargetKeys = [];
        let tempMockData = [];

        const response = await fetch(`https://localhost:7018/api/users/all`, {
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
        })
        if (response.status == 200) {
            const json = await response.json();
            setOtherUser(json)
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
        let number = otherUser.length
        for (let i = 0; i < number; i++) {
            const data = {
                key: (otherUser[i].id),
                title: otherUser[i].login,
            };
            //if (arr.includes(otherUser[i].id)) tempTargetKeys.push(data.key);

            tempMockData.push(data);
        }
        setMockDataCreate(tempMockData);
        //setMockData(tempMockData);
    }
    async function getPartisipantsAndOtherUser(groupId) {

        let tempTargetKeys = [];
        let tempMockData = [];
        /*for (let i = 0; i < 20; i++) {
            const data = {
                key: i.toString(),
                title: `content${i + 1}`,
               
                
            };
            
                tempTargetKeys.push(data.key);
            
            tempMockData.push(data);
        }
        setMockData(tempMockData);
        setTargetKeys(tempTargetKeys);*/

        
        const response1 = await fetch(`https://localhost:7018/api/groups/${groupId}`, {
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
        })
        if (response1.status == 200) {
            const json = await response1.json();
            setParticipantsGroup(json)
        }
        const response2 = await fetch('https://localhost:7018/api/users/all', {
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include'
        })
        if (response2.status == 200) {
            const json = await response2.json();
        
            setOtherUser(json);
        }

        let arr = []
        let noUser = []
       
        for (let b of participantsGroup) {
            arr.push(b.id);
        }
        for (let a of otherUser) {
            if (!arr.includes(a.id)) noUser.push(a) 
        }
        /*console.log(arr)
        console.log(noUser)*/

        let number = otherUser.length
        for (let i = 0; i < number; i++) {
            const data = {
                key: (otherUser[i].id),
                title: otherUser[i].login,
            };
            if (arr.includes(otherUser[i].id)) tempTargetKeys.push(data.key);

            tempMockData.push(data);
        }
        setMockData(tempMockData);
        setTargetKeys(tempTargetKeys);
        
    }

    const handleChange = (newTargetKeys) => {
        setTargetKeys(newTargetKeys);
    };
    const handleChangeCreate = (newTargetKeys) => {
        setTargetKeysCreate(newTargetKeys)
    }

    async function handleOkParticipants(item) {

        const response = await fetch('https://localhost:7018/api/groups/usergroup', {
            method: 'PATCH',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                groupId: item.id,
                usersId: targetKeys,
            })
        })
        if (response.status == 200) {
            messageApi.open({
                type: 'success',
                content: 'Состав группы изменен',
            });
            setIsShowModalParticipants(false)
        }
        
    }

    async function handleOkRename(item) {
        setItem(item);
        const newName = document.querySelector('#newName').value
        const response = await fetch('https://localhost:7018/api/groups', {
            method: 'PATCH',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                id: item.id,
                name: newName,
            })
        })
        if (response.status == 200) {
            const index = listGroup.findIndex((el) => el.id == item.id);
            const newList = listGroup.filter((itemList) => itemList.id !== item.id);
            newList.splice(index, 0, new Object({ id: item.id, name: newName }))
            const newListGroup = listGroup;
            setListGroup(newList);
            messageApi.open({
                type: 'success',
                content: 'Название группы изменено',
            });
            setIsShowModalRename(false);
        }
        
    }

    async function handleOkDelete(item) {
        setItem(item);

        const response = await fetch(`https://localhost:7018/api/groups/${item.id}`, {
            method: 'DELETE',
            headers: new Headers({ "Content-Type": "application/json" }),
            
        })
        if (response.status == 200) {
            const newList = listGroup.filter((itemList) => itemList.id !== item.id);
            setListGroup(newList);
            messageApi.open({
                type: 'success',
                content: 'Группа удалена',
            });
            setIsShowModalDelete(false);
        }
        
    }

    async function handleOkCreate() {
        const response = await fetch('https://localhost:7018/api/groups', {
            method: 'POST',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                name: document.querySelector('#Name').value,
                participants: targetKeysCreate,
            })
        })
        if (response.status == 201) {
            const json = await response.json()
            setListGroup([...listGroup, json])
            messageApi.open({
                type: 'success',
                content: 'Группа добавлена',
            });
            
            setIsShowModalCreate(false)
        }
        
    }
    async function getListGroup() {
        const response = await fetch('https://localhost:7018/api/groups/all', {
            headers: new Headers({ "Content-Type": "application/json" }),
        });
        if (response.status == 200) {
            
            const json = await response.json();
            setListGroup(json);
        }
        
    }

    useEffect(() => {
        getListGroup();
        getAll();
        //getPartisipantsAndOtherUser();
    }, [])

    


    return (
        <>
            {contextHolder}
            <List
                header={<div style={{ textAlign: 'right', fontSize: 20 }}><a onClick={showModalCreate}><UsergroupAddOutlined /> Добавить</a></div>}

                style={{ width: 700 }}
                dataSource={listGroup}
                renderItem={(item) => (
                    <List.Item style={{ marginLeft: '10px', textAlign: 'left', fontSize: '20px' }}
                        actions={[<a onClick={() => showModalRename(item)}>Переименовать</a>, <a onClick={() => showModalDelete(item)}>Удалить</a>,
                            <a onClick={() => showModalParticipants(item)}>Отредактировать состав</a>]}
                    >{item.name}
                    </List.Item >
                )}
            />
            <Modal title='Введите новое название группы' open={isShowModalRename} onOk={() => handleOkRename()}  onCancel={() => handleCancel()} footer={[
                <Button onClick={() => handleCancel()}>Отменить</Button>, <Button onClick={() => handleOkRename(item)}>Изменить название</Button>
            ] }>
                <Input id='newName' defaultValue={item.name} />
            </Modal>
            <Modal title='Вы действительно хотите удалить группу?' open={isShowModalDelete} onOk={() => handleOkDelete()} onCancel={() => handleCancel()} footer={[
                <Button onClick={() => handleCancel()}>Отменить</Button>, <Button onClick={() => handleOkDelete(item)}>Удалить</Button>
            ]}>
                
            </Modal>
            
            <Modal title='Редактирование участников группы' open={isShowModalParticipants} onOk={() => handleOkParticipants()} onCancel={() => handleCancel()} footer={[
                <Button onClick={() => handleCancel()}>Отменить</Button>, <Button onClick={() => handleOkParticipants(item)}>Сохранить</Button>
            ]}>
                
                <Transfer showSearch
                    dataSource={mockData}
                    targetKeys={targetKeys}
                    listStyle={{ width: "1000px", textAlign: 'left' }}
                    locale={{ itemUnit: '', itemsUnit: '' }}
                    titles={["Остальные пользователи", "Участники группы"]}

                    onChange={handleChange}
                    render={(item) => item.title}
                />
            </Modal>
            <Modal title='Добавление группы' open={isShowModalCreate} onOk={() => handleOkCreate()} onCancel={() => handleCancel()} footer={[
                <Button onClick={() => handleCancel()}>Отменить</Button>, <Button onClick={() => handleOkCreate()}>Сохранить</Button>
            ]}>
                <Form>
                    <Form.Item label="Название"
                        name="Name"
                        rules={[
                            {
                                required: true,
                                message: 'Введите название группы пользователей',
                            },
                        ]}>
                        <Input id='Name' />
                    </Form.Item>
                </Form>
                <Transfer showSearch
                    listStyle={{ width: "1000px", textAlign: 'left' }}
                    locale={{ itemUnit: '', itemsUnit: '' }}
                    titles={["Остальные пользователи", "Участники группы"]}
                    dataSource={mockDataCreate}
                    targetKeys={targetKeysCreate}
                    onChange={handleChangeCreate}
                    render={(item) => item.title}
                />
            </Modal>
        </>
    )
}

export default ListGroupComponent;