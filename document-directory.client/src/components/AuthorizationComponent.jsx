import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Input, Button } from 'antd';
import { FolderFilled, FolderAddFilled, FileAddFilled, FileFilled, CloseOutlined, DeleteFilled } from '@ant-design/icons';

function Authorization() {
    const [user, setUser] = useState({});
    const navigate = useNavigate();

    async function authorization() {
        const login = document.querySelector('#inputLogin').value;
        const password = document.querySelector('#inputPassword').value;
        const response = await fetch('https://localhost:7018/api/authorization', {
            method: 'POST',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                Login: login,
                Password: password
            })
        });
        if (response.status == 200) {
            const json = await response.json();
            setUser(json);
            navigate('/');
        }
    }

    return (
        <>
                �����������
            <div style={{margin: '10px'} }>
                <Input placeholder="������� �����" id="inputLogin" style={{ width: '50%' }} />
            </div>
            <div style={{ margin: '10px' }}>
                <Input placeholder="������� ������" id="inputPassword" style={{ width: '50%' }} />
            </div>
            <div>
                <Button size='small' onClick={authorization} style={{ width: "100px" }}>���������</Button>
            </div>
            
        </>
    )
}

export default Authorization;