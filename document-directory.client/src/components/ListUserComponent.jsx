import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { message, Button, List, Select, Modal } from 'antd';

function ListUserComponent() {
    const [list, setList] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [item, setItem] = useState({});
    const [roles, setRoles] = useState([]);
    const navigate = useNavigate();
    const [messageApi, contextHolder] = message.useMessage();
    
    

    const showModal = (item) => {
        setItem(item)
        setIsModalOpen(true);
    };
    async function handleOk() {
        const response = await fetch(`https://localhost:7018/api/users/${item.id}`, {
            method: 'DELETE',
            headers: new Headers({ "Content-Type": "application/json" }),
        });
        if (response.status == 200) {
            const newList = list.filter((itemList) => itemList.id !== item.id);
            setList(newList);
            messageApi.open({
                type: 'success',
                content: 'Пользователь удален',
            });
        }
        setIsModalOpen(false);
    };
    const handleCancel = () => {
        setIsModalOpen(false);
    };

    async function handleChange(value, item) {
        setItem(item);
        item.roleId = value
        const response = await fetch('https://localhost:7018/api/users', {
            method: 'PATCH',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                id: item.id,
                roleId: item.roleId,
            })
        });
        if (response.status == 200) {
            messageApi.open({
                type: 'success',
                content: 'Роль пользователя изменена',
            });
        }
        
    };

    async function getAllUsers() {
        const response = await fetch('https://localhost:7018/api/users/all', {
            method: 'GET',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        const json = await response.json();
        setList(json);
        
    }

    

    async function getRoles() {
        const response = await fetch('https://localhost:7018/api/role/all', {
            method: 'GET',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        const json = await response.json();
        setRoles(json);
    }

    

    useEffect(() => {
        getAllUsers();
        getRoles();
    }, []);
        
    

    return (
        <>
            {contextHolder}
            <List 
                style={{ width: 700 }}
                dataSource={list}

                renderItem={(item) => (
                    <List.Item style={{ marginLeft: '10px', textAlign: 'left' }}
                        actions={[<Select style={{ width: '145px' }}
                            defaultValue={item.roleId}
                            onChange={(e) => handleChange(e, item)}
                            options={[
                                {
                                    value: 1,
                                    label: "Администратор",
                                },
                                {
                                    value: 2,
                                    label: "Пользователь",
                                },
                            ]
                            }
                        />, <a onClick={() => console.log(item.id)}>Сменить пароль</a>, <a onClick={() => showModal(item)}>Удалить</a>,]}
                    >{item.login} 

                    </List.Item>
                )}
            />
            <Modal title="Вы действительно хотите удалить пользователя?" open={isModalOpen} onOk={handleOk} onCancel={handleCancel} footer={[
                <Button onClick={handleCancel}>Отменить</Button>, <Button onClick={handleOk}>Удалить</Button>
            ] }>
                <p>{ }</p>
                
            </Modal>
        </>
    );
    
    
}
export default ListUserComponent;